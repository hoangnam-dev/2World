using _2World.Data;
using _2World.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.IO;


namespace _2World.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private int PageSize = 10;

        public ProductController(AppDbContext _context, IWebHostEnvironment webHostEnvironment)
        {
            context = _context;
            this.webHostEnvironment = webHostEnvironment;
        }
        // GET: ProductController
        [HttpGet]
        public ActionResult Index(string? searchKey, int page = 1)
        {
            IQueryable<Product> products = context.Products.Where(p => p.Deleted_At == null).OrderByDescending(p => p.Updated_At);

            if (!string.IsNullOrEmpty(searchKey))
            {
                products = products.Where(p => p.Name.Contains(searchKey));
            }
            int totalItem = products.Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)totalItem / PageSize);
            ViewBag.CurrentPage = page;
            products = products.Skip((page - 1) * PageSize).Take(PageSize);

            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"] != null ? TempData["Msg"].ToString() : "";
                ViewBag.Status = TempData["Status"] != null ? TempData["Status"].ToString() : "";
            }

            return View(products.ToList());
        }

        // GET: ProductController/Create
        [HttpGet]
        public ActionResult Create()
        {
            var categories = context.Categories.Where(c => c.Deleted_At == null).ToList();
            ViewBag.Categories = categories;

            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile Image_Path)
        {
            var categories = await context.Categories.Where(c => c.Deleted_At == null).ToListAsync();
            ViewBag.Categories = categories;
            if (ModelState.IsValid)
            {
                if (Image_Path != null && Image_Path.Length > 0)
                {
                    // Tạo đường dẫn cho tệp tin được tải lên
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Image_Path.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Lưu tệp tin vào thư mục uploads
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image_Path.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn tệp tin vào model
                    product.Image_Path = uniqueFileName;
                }

                product.Created_At = DateTime.Now;
                product.Updated_At = DateTime.Now;
                context.Products.Add(product);
                await context.SaveChangesAsync();
                TempData["Msg"] = "Product has been successfully added.";
                TempData["Status"] = "success";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: ProductController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                TempData["Msg"] = "Product not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = context.Categories.Where(c => c.Deleted_At == null).ToList();

            var product = await context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null || ViewBag.Categories == null)
            {
                TempData["Msg"] = "Product not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Name,Price,Quantity,Category_Id,Description, Image_Path,Created_At")] Product product, IFormFile? newImage)
        {
            if (ModelState.IsValid)
            {
                product.Updated_At = DateTime.Now;
                if (newImage != null && newImage.Length > 0)
                {
                    // Tạo đường dẫn cho tệp tin được tải lên
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + newImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Lưu tệp tin vào thư mục uploads
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await newImage.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn tệp tin vào model
                    product.Image_Path = uniqueFileName;
                }
                context.Update(product);
                Console.WriteLine(product.Image_Path);
                await context.SaveChangesAsync();
                ViewBag.Msg = "Product has been successfully edited.";
                ViewBag.Status = "success";
                ViewBag.Categories = context.Categories.Where(c => c.Deleted_At == null).ToList();

                return View(product);
            }
            TempData["Msg"] = "Failed to delete product.";
            TempData["Status"] = "danger";
            return RedirectToAction(nameof(Index));
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int itemId)
        {
            try
            {
                var product = await context.Products.FindAsync(itemId);
                DateTime originalCreatedAt = product.Created_At;
                product.Created_At = originalCreatedAt;
                product.Updated_At = DateTime.Now;
                product.Deleted_At = DateTime.Now;
                await context.SaveChangesAsync();
                TempData["Msg"] = "Product has been successfully deleted.";
                TempData["Status"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Msg"] = "Failed to delete product.";
                TempData["Status"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Export()
        {
            var products = context.Products
                .Where(p => p.Deleted_At == null)
                .Join(
                    context.Categories,
                    product => product.Category_Id,
                    category => category.Id,
                    (product, category) => new
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Image = product.Image_Path,
                        Price = product.Price,
                        Quantity = product.Quantity,
                        Created_At = product.Created_At,
                        Updated_At = product.Updated_At,
                        CategoryName = category.Name
                    }
                )
                .ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // Định dạng header
                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Name";
                worksheet.Cells["C1"].Value = "Description";
                worksheet.Cells["D1"].Value = "Price";
                worksheet.Cells["E1"].Value = "Quantity";
                worksheet.Cells["F1"].Value = "Category";
                worksheet.Cells["G1"].Value = "Created at";
                worksheet.Cells["H1"].Value = "Updated at";
                worksheet.Cells["I1"].Value = "Image";

                // Đổ dữ liệu vào từng ô
                var dateTimeFormat = "dd/MM/yyyy HH:mm:ss";

                int row = 2;
                foreach (var product in products)
                {
                    worksheet.Cells[string.Format("A{0}", row)].Value = product.Id;
                    worksheet.Cells[string.Format("B{0}", row)].Value = product.Name;
                    worksheet.Cells[string.Format("C{0}", row)].Value = product.Description;
                    worksheet.Cells[string.Format("D{0}", row)].Value = product.Price;
                    worksheet.Cells[string.Format("E{0}", row)].Value = product.Quantity;
                    worksheet.Cells[string.Format("F{0}", row)].Value = product.CategoryName;
                    worksheet.Cells[string.Format("G{0}", row)].Value = product.Created_At;
                    worksheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = dateTimeFormat;
                    worksheet.Cells[string.Format("H{0}", row)].Value = product.Updated_At;
                    worksheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = dateTimeFormat;
                    worksheet.Cells[string.Format("I{0}", row)].Value = product.Image ?? "N/A";
                    row++;
                }

                // Chuyển MemoryStream thành byte array
                byte[] data = package.GetAsByteArray();

                // Trả về file Excel
                return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
            }
        }
    }
}
