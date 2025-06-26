using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCtesting.Models;
using MVCtesting.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace MVCtesting.Controllers
{
    public class CustomerCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerCategoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Show product detail and order form
        public IActionResult OrderProduct(int id)
        {
            var product = _context.Tproducts
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefault(p => p.product_id == id);

            if (product == null)
            {
                return NotFound();
            }

            var model = new OrderViewModel
            {
                ProductId = product.product_id,
                ProductName = product.Name,
                BrandName = product.Brand?.Name,
                CategoryName = product.Category?.Name,
                Price = product.Price.Value,
                Description = product.Description,
                ImageFileName = product.ImageFileName,
                Quantity = 1 // default quantity
            };

            return View("Order", model); // assuming your view is Order.cshtml
        }


        // Handle order form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitOrder(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("OrderProduct", model); // or return the same view
            }

            var product = await _context.Tproducts.FirstOrDefaultAsync(p => p.Id == model.ProductId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index");
            }

            // ✅ Your main order creation logic goes here
            var order = new Order
            {
                CustomerId = _userManager.GetUserId(User),
                OrderDate = DateTime.Now,
                Status = "Pending",
                PaymentMethod = model.PaymentMethod,
                OrderItems = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                Price = product.Price ?? 0

            }
        }
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Order placed for {product.Name} (Qty: {model.Quantity})";

            // ✅ Redirect to MyOrders in OrderController
            return RedirectToAction("MyOrders", "Order");
        }


        // 1. Show all categories
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // 2. Show brands and products under selected category
        public IActionResult SeeMore(int id)
        {
            if (id == 0)
                return BadRequest("Category id is required.");

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound("Category not found.");

            var brands = _context.CategoryBrands
                         .Where(cb => cb.CategoryId == id)
                         .Select(cb => cb.Brand)
                         .Distinct()
                         .ToList();

            var products = _context.Tproducts
                            .Include(p => p.Brand)
                            .Where(p => p.CategoryId == id)
                            .ToList();

            var model = new SeeMoreViewModel
            {
                Category = category,
                Brands = brands,
                Products = products
            };

            return View(model);
        }

        // 4. Quick order (fallback)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeOrder(int productId, int quantity)
        {
            if (quantity < 1)
            {
                TempData["ErrorMessage"] = "Quantity must be at least 1.";
                var categoryId = await _context.Tproducts
                    .Where(p => p.Id == productId)
                    .Select(p => p.CategoryId)
                    .FirstOrDefaultAsync();
                return RedirectToAction("SeeMore", new { id = categoryId });
            }

            var product = await _context.Tproducts
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index");
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "You must be logged in to place an order.";
                return RedirectToAction("Login", "Account");
            }

            if (product.Price == null || product.Price == 0)
            {
                TempData["ErrorMessage"] = "Product price is not set.";
                return RedirectToAction("SeeMore", new { id = product.CategoryId });
            }

            var order = new Order
            {
                ProductId = productId,
                ProductName = product.Name,
                Quantity = quantity,
                Price = product.Price.Value,  // <-- Use .Value because Price is nullable
                OrderDate = DateTime.Now,
                CustomerId = userId,
                PaymentMethod = "Cash",
                Status = "Pending"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                Order = order,
                Product = product,
                ProductId = product.Id,
                Price = product.Price.Value,
                Quantity = quantity

            };

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Order placed for {product.Name} (Qty: {quantity})";
            return RedirectToAction("SeeMore", new { id = product.CategoryId });
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Where(o => o.CustomerId == user.Id)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders); // This will use MyOrders.cshtml
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == user.Id && o.Status == "Pending");

            if (order == null)
                return NotFound();

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order cancelled.";
            return RedirectToAction("MyOrders");
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == user.Id);

            if (order == null)
                return NotFound();

            return View(order); // You can create Details.cshtml
        }



    }
}
