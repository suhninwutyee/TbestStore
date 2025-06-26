using Microsoft.AspNetCore.Mvc;
using MVCtesting.Models;
using MVCtesting.Services;

namespace MVCtesting.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Settings
        public IActionResult Index()
        {
            var settings = _context.ApplicationSettings.FirstOrDefault()
                           ?? new ApplicationSetting(); // fallback if no record
            return View(settings);
        }

        // POST: Settings
        [HttpPost]
        public IActionResult Index(ApplicationSetting model)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.ApplicationSettings.FirstOrDefault();
                if (existing == null)
                {
                    _context.ApplicationSettings.Add(model);
                }
                else
                {
                    existing.SiteName = model.SiteName;
                    existing.IsMaintenanceMode = model.IsMaintenanceMode;
                    existing.SupportEmail = model.SupportEmail;

                    _context.ApplicationSettings.Update(existing);
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Settings updated successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
