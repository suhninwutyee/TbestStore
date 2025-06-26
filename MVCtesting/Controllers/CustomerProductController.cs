using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCtesting.Models;
using MVCtesting.Services;
using System.Linq;

namespace MVCtesting.Controllers
{
    public class CustomerProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CustomerProduct/Details/5
        public IActionResult Details(int id)
        {
            var product = _context.Tproducts
                .Include(p => p.Brand)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
