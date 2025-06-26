using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCtesting.Services;

namespace MVCtesting.Controllers
{
    [Authorize] // Ensure only logged-in users can access
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var productCount = _context.Tproducts.Count();
            var categoryCount = _context.Categories.Count();
            var brandCount = _context.Brands.Count();

            ViewBag.ProductCount = productCount;
            ViewBag.CategoryCount = categoryCount;
            ViewBag.BrandCount = brandCount;

            return View();
        }
    }
}
