using _2World.Data;
using _2World.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2World.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext context;
        protected int PageSize = 10;

        public UserController(AppDbContext _context)
        {
            this.context = _context;
        }
        // GET: UserController
        public ActionResult Index(string? searchKey, int page = 1)
        {
            int totalItem = context.Users.Where(u => u.Deleted_At == null).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)totalItem / PageSize);
            ViewBag.CurrentPage = page;

            IQueryable<User> users = context.Users.Where(u => u.Deleted_At == null).Skip((page - 1) * PageSize).Take(PageSize).OrderByDescending(u => u.Created_At);

            if (!string.IsNullOrEmpty(searchKey))
            {
                int userId;
                bool isUserId = int.TryParse(searchKey, out userId);

                if (isUserId)
                {
                    users = users.Where(o => o.Id == userId);
                }
                else
                {
                    users = users.Where(c => c.Name.Contains(searchKey));
                }
            }

            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"] != null ? TempData["Msg"].ToString() : "";
                ViewBag.Status = TempData["Status"] != null ? TempData["Status"].ToString() : "";
            }
            return View(users.ToList());
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            ViewBag.Roles = context.Roles.Where(r => r.Deleted_At == null).ToList();
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include: "Name,Email,Phone,Password,Role_Id")] User user, string reTypePw)
        {
            ViewBag.Roles = await context.Roles.Where(r => r.Deleted_At == null).ToListAsync();
            if (ModelState.IsValid)
            {
                Console.WriteLine(checkEmailExist(user.Email));
                if (checkEmailExist(user.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email already exists.");
                    return View(user);
                }
                if (!user.Password.Equals(reTypePw))
                {
                    ModelState.AddModelError(string.Empty, "Retype password do not match.");
                    return View(user);
                }
                user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, 13);
                user.Created_At = DateTime.Now;
                user.Updated_At = DateTime.Now;
                context.Users.Add(user);
                await context.SaveChangesAsync();
                TempData["Msg"] = "User has been successfully added.";
                TempData["Status"] = "success";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Failed to add user.");
            return View(user);
        }

        // GET: UserController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            int authUserId = int.Parse(HttpContext.Session.GetString("UserId"));
            if (authUserId == id)
            {
                return RedirectToAction("Detail", "Auth", new { id = id });
            }
            ViewBag.Roles = await context.Roles.Where(r => r.Deleted_At == null).ToListAsync();
            if (id == null)
            {
                TempData["Msg"] = "User not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }

            var user = await context.Users.FindAsync(id);

            if (user == null)
            {
                TempData["Msg"] = "User not found.";
                TempData["Status"] = "warning";
                return RedirectToAction(nameof(Index));
            }
            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"] != null ? TempData["Msg"].ToString() : "";
                ViewBag.Status = TempData["Status"] != null ? TempData["Status"].ToString() : "";
            }
            return View(user);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(include: "Id,Name,Email,Phone,Role_Id,Password,Created_At, Updated_Ats")] User user, string? newPass, string? retypePass)
        {
            int authUserId = int.Parse(HttpContext.Session.GetString("UserId"));
            if (authUserId == user.Id)
            {
                return RedirectToAction("Detail", "Auth", new { id = user.Id });
            }
            if (ModelState.IsValid)
            {
                var _user = context.Users.FirstOrDefault(c => c.Id == user.Id);
                if (newPass != null)
                {
                    if (retypePass == null || !newPass.Equals(retypePass))
                    {
                        TempData["Msg"] = "Retype password do not match.";
                        TempData["Status"] = "danger";
                        return RedirectToAction(nameof(Edit), new { id = _user.Id });
                    }
                    _user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPass, 13);
                }

                if (!_user.Email.Equals(user.Email))
                {
                    bool exits = checkEmailExist(user.Email);
                    if (exits)
                    {
                        TempData["Msg"] = "Email already exits.";
                        TempData["Status"] = "danger";
                        return RedirectToAction(nameof(Edit), new { id = _user.Id });
                    }
                    else
                    {
                        _user.Email = user.Email;
                    }
                }
                _user.Updated_At = DateTime.Now;
                context.Update(_user);
                await context.SaveChangesAsync();
                TempData["Msg"] = "User has been successfully edited.";
                TempData["Status"] = "success";
                return RedirectToAction(nameof(Edit), new { id = _user.Id });
            }
            ModelState.AddModelError(string.Empty, "Failed to edit user.");
            return RedirectToAction(nameof(Edit));
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            int authUserId = int.Parse(HttpContext.Session.GetString("UserId"));
            if (authUserId == id)
            {
                TempData["Msg"] = "You can not delete youself.";
                TempData["Status"] = "danger";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var user = await context.Users.FindAsync(id);
                user.Deleted_At = DateTime.Now;
                user.Updated_At = DateTime.Now;
                await context.SaveChangesAsync();
                TempData["Msg"] = "User has been successfully deleted.";
                TempData["Status"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Msg"] = "Failed to delete user.";
                TempData["Status"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }
        public bool checkEmailExist(string email)
        {
            var userHaveEmail = context.Users.Where(u => u.Email.Equals(email)).Count();
            if (userHaveEmail == 0)
            {
                return false;
            }
            return true;
        }
    }
}
