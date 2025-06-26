using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCtesting.Models;
using MVCtesting.Services;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;  // <== Add this

// other usings


namespace MVCtesting.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CustomerController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> ProductList()
        {
            var products = await _context.Tproducts.ToListAsync();
            return View(products);
        }
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            // Add logic to add product to the user's cart
            TempData["SuccessMessage"] = "Product added to cart!";
            return RedirectToAction("AllProducts");
        }
      
        
        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var products = await _context.Tproducts.ToListAsync();
            return View(products);
        }

        // GET: /Customer/MakeOrde      

        // POST: Customer/MakeOrder


        // GET: /Customer/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);

            // Fetch recent orders for this customer
            var orders = await _context.Orders
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Find hot product in last 7 days
            var oneWeekAgo = DateTime.Now.AddDays(-7);
            var hotProduct = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.OrderDate >= oneWeekAgo)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(g => g.QuantitySold)
                .Join(_context.Tproducts,
                      g => g.ProductId,
                      p => p.Id,
                      (g, p) => new
                      {
                          Id = p.Id,
                          Name = p.Name,
                          ImagePath = p.ImageFileName, // Make sure this is the correct property
                          Price = p.Price,
                          QuantitySold = g.QuantitySold
                      })
                .FirstOrDefaultAsync();

            ViewBag.HotProduct = hotProduct;

            return View(orders);
        }



        // GET: /Customer/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // GET: /Customer/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Customer/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Your password has been changed.";
                return RedirectToAction(nameof(ChangePassword));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
    }
}
