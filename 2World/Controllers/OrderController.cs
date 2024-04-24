using _2World.Data;
using _2World.Models;
using _2World.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static NuGet.Packaging.PackagingConstants;
using System.Drawing.Printing;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace _2World.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext context;
        private int PageSize = 10;

        public OrderController(AppDbContext context)
        {
            this.context = context;
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
                orders = orders.Where(o => o.Id == int.Parse(searchKey));
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
                            orderStatus = 1;
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
        public async Task<IActionResult> Cancel(int id)
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
                ViewBag.ErrorMessage = "This order cannot be cancelled because it has already been completed.";
                return View("Error");
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

            TempData["Msg"] = "Order has been successfully canceled.";
            TempData["Status"] = "success";
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
    }
}
