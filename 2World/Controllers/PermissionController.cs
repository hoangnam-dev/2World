using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2World.Controllers
{
    public class PermissionController : Controller
    {
        // GET: PermissionController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PermissionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PermissionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PermissionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PermissionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PermissionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PermissionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PermissionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
