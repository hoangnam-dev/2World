using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using _2World.Data;
using _2World.Models.ViewModels;
using _2World.Models;

namespace _2World.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext context;
        private int PageSize = 10;
        private readonly IConfiguration configuration;
        private readonly ILogger<OrderController> logger;

        public OrderController(AppDbContext context, IConfiguration _configuration, ILogger<OrderController> _logger)
        {
            this.context = context;
            this.configuration = _configuration;
            this.logger = _logger;
        }

        public ActionResult Index(string? searchKey, [FromQuery(Name = "status")] string? status, int page = 1)
        {
            var orders = context.Orders.Join(context.Customers, order => order.Customer_Id, customer => customer.Id, (order, customer) => new OrderViewModel
            {
                Id = order.Id,
                CustomerName = customer.Name,
                CustomerAddress = customer.Address,
                CustomerEmail = customer.Email,
                CustomerPhone = customer.Phone,
                Order_Date = order.Order_Date,
                Delivery_Date = order.Delivery_Date,
                Status = order.Status
            });

            if (!string.IsNullOrEmpty(searchKey))
            {
                int orderId;
                bool isOrderId = int.TryParse(searchKey, out orderId);

                if (isOrderId)
                {
                    orders = orders.Where(o => o.Id == orderId);
                }
                else
                {
                    orders = orders.Where(o => o.CustomerName.Contains(searchKey));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(status))
                {
                    int orderStatus = 0;
                    switch (status)
                    {
                        case "isProgressed":
                            orderStatus = 1;
                            break;
                        case "isCompleted":
                            orderStatus = 2;
                            break;
                        case "isCanceled":
                            orderStatus = 3;
                            break;
                        default:
                            orderStatus = 0;
                            break;
                    }
                    orders = orders.Where(o => o.Status == orderStatus);
                }
            }

            int totalItem = orders.Count();
            int skipItems = (page - 1) * PageSize;
            orders = orders
                    .OrderByDescending(o => o.Order_Date)
                    .Skip(skipItems)
                    .Take(PageSize);

            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"] != null ? TempData["Msg"].ToString() : "";
                ViewBag.Status = TempData["Status"] != null ? TempData["Status"].ToString() : "";
            }
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)totalItem / PageSize);
            ViewBag.CurrentPage = page;

            var orderViewModel = orders.ToList();

            return View(orderViewModel);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                TempData["Msg"] = "Order not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }

            var order = (from item in context.Orders
                         join customer in context.Customers on item.Customer_Id equals customer.Id
                         where item.Id == id
                         select new OrderViewModel
                         {
                             Id = item.Id,
                             CustomerId = customer.Id,
                             CustomerName = customer.Name,
                             CustomerAddress = customer.Address,
                             CustomerEmail = customer.Email,
                             CustomerPhone = customer.Phone,
                             Order_Date = item.Order_Date,
                             Delivery_Date = item.Delivery_Date,
                             Status = item.Status,
                             TotalAmount = context.OrderItems
                                                 .Where(oi => oi.Order_Id == item.Id)
                                                 .Sum(oi => oi.Quantity * oi.Price),
                             OrderItems = context.OrderItems
                                                 .Where(oi => oi.Order_Id == item.Id)
                                                 .Select(oi => new OrderItemViewModel
                                                 {
                                                     Order_Id = oi.Order_Id,
                                                     Product_Id = oi.Product_Id,
                                                     ProductName = oi.Product.Name,
                                                     Price = oi.Price,
                                                     Quantity = oi.Quantity
                                                 })
                                                 .ToList()
                         }).FirstOrDefault();

            if (order == null)
            {
                TempData["Msg"] = "Order not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int status, string? delivery)
        {
            if (id == null)
            {
                TempData["Msg"] = "Order not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }
            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                TempData["Msg"] = "Order not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }

            if (status == 0 && !string.IsNullOrEmpty(delivery))
            {
                DateTime orderDelivery = DateTime.Parse(delivery);
                Console.WriteLine(orderDelivery);
                if (checkDeliveryDate(order.Order_Date, orderDelivery))
                {
                    order.Delivery_Date = orderDelivery;
                    order.Status = 1;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The delivery date cannot be earlier than the order date.");
                    return View(order);
                }
            }

            if (status == 1)
            {
                order.Status = 2;
            }

            await context.SaveChangesAsync();
            ViewBag.Msg = "Order has been successfully edited.";
            ViewBag.Status = "success";
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id, string customerEmail, string mailBody)
        {
            var order = await context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                TempData["Msg"] = "Order not found.";
                TempData["Status"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            if (order.Status == 2)
            {
                TempData["Msg"] = "This order cannot be cancelled because it has already been completed.";
                TempData["Status"] = "danger";
                return RedirectToAction(nameof(Edit), new {id = id});
            }

            foreach (var item in order.OrderItems)
            {
                var product = await context.Products.FindAsync(item.Product_Id);
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                    context.Update(product);
                }
            }


            order.Status = 3;
            context.Update(order);
            await context.SaveChangesAsync();

            if (!SendEmail(customerEmail, "Cancel Order", id, mailBody))
            {
                TempData["Msg"] = "Can not send email to Customer. Order has been successfully canceled.";
                TempData["Status"] = "warning";
            }
            else
            {
                TempData["Msg"] = "Order has been successfully canceled.";
                TempData["Status"] = "success";
            }
            return RedirectToAction(nameof(Index));
        }

        public bool checkDeliveryDate(DateTime orderDate, DateTime deliveryDate)
        {
            if (orderDate >= deliveryDate)
            {
                return false;
            }
            return true;
        }

        public bool SendEmail(string toEmail, string subject, int orderId, string body = "Shipping failed. There is no one to receive the order")
        {
            string SMPTServer = configuration["MailSettings:Server"];
            int Port = int.Parse(configuration["MailSettings:Port"]);
            string SenderName = configuration["MailSettings:SenderName"];
            string SenderEmail = configuration["MailSettings:SenderEmail"];
            string WebEmail = configuration["MailSettings:Email"];
            string EmailPassword = configuration["MailSettings:Pasword"];
            try
            {
                // Create message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(SenderName, SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                // Create mail body
                var bodyBuilder = new BodyBuilder();    
                bodyBuilder.HtmlBody = $"<h1>Order Cancellation</h1>" +
                    $"<p>We regret to inform you that your order with reference number <strong style=\"color: #FF3030;font-size: 1rem;\">{ orderId }</strong> has been cancelled.</p>" +
                    $"<p>Reason: <strong>{body}</strong></p>"+
                    $"<p>If you have any questions or concerns, please feel free to contact our customer service.</p>" +
                    $"<p>Thank you for choosing our services.</p>";
                message.Body = bodyBuilder.ToMessageBody();

                // Setting SMTP info
                using var client = new SmtpClient();
                client.Connect(SMPTServer, Port, SecureSocketOptions.StartTls);
                client.Authenticate(WebEmail, EmailPassword);

                // Send mail
                client.Send(message);
                client.Disconnect(true);
                return true;
            }
            catch
            {
                logger.LogError("Cannot send email.");
                return false;
            }
        }
    }
}
