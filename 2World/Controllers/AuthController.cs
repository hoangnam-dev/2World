using _2World.Data;
using _2World.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2World.Models.ViewModels;
using Microsoft.CodeAnalysis.Differencing;


namespace _2World.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext context;

        public AuthController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User _user)
        {
            try
            {
                var user = context.Users.FirstOrDefault(u => u.Email.Equals(_user.Email) && u.Deleted_At == null);
                if (user != null)
                {
                    bool checkPW = BCrypt.Net.BCrypt.EnhancedVerify(_user.Password, user.Password);
                    if (checkPW)
                    {
                        HttpContext.Session.SetString("UserId", Convert.ToString(user.Id));
                        HttpContext.Session.SetString("UserName", user.Name);
                        HttpContext.Session.SetString("UserRole", Convert.ToString(user.Role_Id));
                        var role = context.Roles.FirstOrDefault(r => r.Id == user.Role_Id);
                        HttpContext.Session.SetString("RoleName", role.Name);
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError(string.Empty, "Invalid password.");
                    return View();
                }
                ModelState.AddModelError(string.Empty, "Invalid email.");
                return View(_user);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Have error can not login.");
                return View(_user);
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var user = await context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(); // Trả về NotFound nếu không tìm thấy người dùng
            }

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Password = user.Password,
                RoleId = user.Role.Id,
                RoleName = user.Role.Name
            };
            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"] != null ? TempData["Msg"].ToString() : "User info updated.";
                ViewBag.Status = TempData["Status"] != null ? TempData["Status"].ToString() : "success";
            }
            return View(userViewModel);
        }
        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail([Bind(include: "Id, Name, Email, Phone, Password, RoleId, RoleName")] UserViewModel _user)
        {
            if (ModelState.IsValid)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == _user.Id);
                if (!user.Email.Equals(_user.Email))
                {
                    bool exits = checkEmailExist(_user.Email);
                    if (exits)
                    {
                        TempData["Msg"] = "Email already exits.";
                        TempData["Status"] = "danger";
                        return RedirectToAction(nameof(Detail), new { id = _user.Id });
                    }
                    else
                    {
                        user.Email = _user.Email;
                    }
                }
                user.Name = _user.Name;
                user.Phone = _user.Phone;
                user.Updated_At = DateTime.Now;
                context.Users.Update(user);
                await context.SaveChangesAsync();
                ViewBag.Msg = "User info updated.";
                ViewBag.Status = "success";
                return View(_user);
            }
            ViewBag.Msg = "User can not update.";
            ViewBag.Status = "danger";
            return View(_user);
        }

        // POST: UserController/ChangePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int userId, string oldPass, string newPass, string retypePass)
        {
            if (!retypePass.Equals(newPass))
            {
                TempData["Msg"] = "New password and retype password do not match.";
                TempData["Status"] = "danger";
                return RedirectToAction(nameof(Detail), new { id = userId });
            }
            try
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (BCrypt.Net.BCrypt.EnhancedVerify(oldPass, user.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPass, 13);
                    user.Updated_At = DateTime.Now;
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
                    TempData["Msg"] = "User password updated.";
                    TempData["Status"] = "success";
                    return RedirectToAction(nameof(Detail), new { id = userId });
                }
                TempData["Msg"] = "Current password and new password do not match.";
                TempData["Status"] = "danger";
                return RedirectToAction(nameof(Detail), new { id = userId });
            }
            catch
            {
                TempData["Msg"] = "Have error. Can not cahnge password";
                TempData["Status"] = "danger";
                return RedirectToAction(nameof(Detail), new { id = userId });
            }

        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
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
