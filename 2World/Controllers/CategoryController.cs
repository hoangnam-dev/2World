using _2World.Data;
using _2World.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace _2World.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext context;
        private int PageSize = 10;

        public CategoryController(AppDbContext _context)
        {
            this.context = _context;
        }
        // GET: CategoryController
        public ActionResult Index(string? searchKey, int page = 1)
        {
            IQueryable<Category> categories = context.Categories.Where(p => p.Deleted_At == null).OrderByDescending(c => c.Created_At);

            if (!string.IsNullOrEmpty(searchKey))
            {
                int customerId;
                bool isCategoryId = int.TryParse(searchKey, out customerId);

                if (isCategoryId)
                {
                    categories = categories.Where(c => c.Id == customerId);
                }
                else
                {
                    categories = categories.Where(c => c.Name.Contains(searchKey));
                }
            }

            int totalItem = context.Categories.Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)totalItem / PageSize);
            ViewBag.CurrentPage = page;

            categories = categories.Skip((page - 1) * PageSize).Take(PageSize);

            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"] != null ? TempData["Msg"].ToString() : "";
                ViewBag.Status = TempData["Status"] != null ? TempData["Status"].ToString() : "";
            }

            return View(categories.ToList());
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Created_At = DateTime.Now;
                category.Updated_At = DateTime.Now;
                context.Categories.Add(category);
                await context.SaveChangesAsync();
                TempData["Msg"] = "Category has been successfully added.";
                TempData["Msg"] = "success";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Failed to add category.");
            return View(category);
        }

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                TempData["Msg"] = "Category not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }

            var category = context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                TempData["Msg"] = "Category not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Name, Created_At, Updated_At")] Category category)
        {
            if (ModelState.IsValid)
            {
                DateTime createdAt = category.Created_At;
                category.Created_At = createdAt;
                category.Updated_At = DateTime.Now;

                context.Categories.Update(category);
                await context.SaveChangesAsync();
                ViewBag.Msg = "Category has been successfully edited.";
                ViewBag.Status = "success";
                return View(category);
            }
            ModelState.AddModelError(string.Empty, "The category edit was unsuccessful.");
            return View(category);
        }


        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int itemId)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == itemId);

                DateTime createdAt = category.Created_At;
                category.Created_At = createdAt;
                category.Updated_At = DateTime.Now;
                category.Deleted_At = DateTime.Now;
                await context.SaveChangesAsync();

                var productsInCategory = await context.Products.Where(p => p.Category_Id == itemId && p.Deleted_At == null).ToListAsync();
                foreach (var product in productsInCategory)
                {
                    DateTime created_at = product.Created_At;
                    product.Created_At = created_at;
                    product.Updated_At = DateTime.Now;
                    product.Deleted_At = DateTime.Now;
                }
                TempData["Msg"] = ViewBag.Msg = "Category has been successfully deleted.";
                TempData["Status"] = "success";
                await context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Msg"] = "Failed to delete category.";
                TempData["Status"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
