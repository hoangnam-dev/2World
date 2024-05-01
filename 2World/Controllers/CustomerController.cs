using _2World.Data;
using _2World.Models;
using Microsoft.AspNetCore.Mvc;

namespace _2World.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext context;
        private int PageSize = 10;

        public CustomerController(AppDbContext _context)
        {
            this.context = _context;
        }
        // GET: CustomerController
        public ActionResult Index(string? searchKey, int page = 1)
        {
            IQueryable<Customer> customers = context.Customers.Where(c => c.Deleted_At == null).OrderByDescending(c => c.Created_At);

            if (!string.IsNullOrEmpty(searchKey))
            {
                int customerId;
                bool isCustomerId = int.TryParse(searchKey, out customerId);
                
                if (isCustomerId)
                {
                    customers = customers.Where(c => c.Id == customerId);
                }
                else
                {
                    customers = customers.Where(c => c.Name.Contains(searchKey));
                }
            }
            int totalItem = customers.Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)totalItem / PageSize);
            ViewBag.CurrentPage = page;
            customers = customers.Skip((page - 1) * PageSize).Take(PageSize);
            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"] != null ? TempData["Msg"].ToString() : "";
                ViewBag.Status = TempData["Status"] != null ? TempData["Status"].ToString() : "";
            }
            return View(customers.ToList());
        }

        // GET: CustomerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CustomerController/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include: "Name,Email,Phone,Address,Password")] Customer customer, string reTypePw)
        {
            if (ModelState.IsValid)
            {
                if (checkEmailExist(customer.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email already exists.");
                    return View(customer);
                }
                if (!reTypePw.Equals(customer.Password))
                {
                    ModelState.AddModelError(string.Empty, "Retype password do not match.");
                    return View(customer);
                }
                string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(customer.Password, 13);
                customer.Password = passwordHash;
                customer.Created_At = DateTime.Now;
                customer.Updated_At = DateTime.Now;
                await context.Customers.AddAsync(customer);
                await context.SaveChangesAsync();
                TempData["Msg"] = "Customer has been successfully added.";
                TempData["Status"] = "success";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Failed to add customer.");
            return View(customer);
        }

        // GET: CustomerController/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                TempData["Msg"] = "Customer not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }

            var customer = context.Customers.FirstOrDefault(c => c.Id == id);

            if (customer == null)
            {
                TempData["Msg"] = "Customer not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }
            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"] != null ? TempData["Msg"].ToString() : "";
                ViewBag.Status = TempData["Status"] != null ? TempData["Status"].ToString() : "";
            }
            return View(customer);
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(include: "Id,Name,Email,Phone,Address,Password,Created_At")] Customer customer, string? newPass, string? retypePass)
        {
            if (ModelState.IsValid)
            {
                if (newPass != null && retypePass != null && newPass.Equals(retypePass))
                {
                    customer.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPass, 13);
                }
                var _customer = context.Customers.FirstOrDefault(c => c.Id == customer.Id);
                if (!_customer.Email.Equals(customer.Email))
                {
                    bool exits = checkEmailExist(customer.Email);
                    if (exits)
                    {
                        ViewBag.Msg = "Email already exits.";
                        ViewBag.Status = "danger";
                        return View(_customer);
                    }
                    else
                    {
                        _customer.Email = customer.Email;
                    }
                }
                _customer.Updated_At = DateTime.Now;
                context.Update(_customer);
                await context.SaveChangesAsync();
                ViewBag.Msg = "Customer has been successfully edited.";
                ViewBag.Status = "success";
                return View(_customer);
            }
            ModelState.AddModelError(string.Empty, "Failed to edit customer.");
            return View(customer);
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int itemId)
        {
            try
            {
                var customer = await context.Customers.FindAsync(itemId);
                DateTime createdAt = customer.Created_At;
                customer.Created_At = createdAt;
                customer.Updated_At = DateTime.Now;
                customer.Deleted_At = DateTime.Now;
                await context.SaveChangesAsync();
                TempData["Msg"] = "Customer has been successfully deleted.";
                TempData["Status"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Msg"] = "Failed to delete customer.";
                TempData["Status"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        public bool checkEmailExist(string email)
        {
            var customer = context.Customers.Where(u => u.Email.Equals(email));
            if (customer.Count() == 0)
            {
                return false;
            }
            return true;
        }
    }
}
