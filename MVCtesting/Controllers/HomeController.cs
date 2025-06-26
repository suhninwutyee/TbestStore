using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCtesting.Models;
using MVCtesting.Services;

namespace MVCtesting.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        // Inject both logger and db context
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Open to everyone (no login required)
        public IActionResult Index()
        {
            return View();
        }

        // Open to everyone
        public IActionResult Privacy()
        {
            return View();
        }

        // Admin only
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            // You can add any admin-specific logic here, e.g. querying admin-only data
            return View();
        }

        // User only
        [Authorize(Roles = "User")]
        public IActionResult UserDashboard()
        {
            // You can add user-specific logic here
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
