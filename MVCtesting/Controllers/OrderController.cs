using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCtesting.Models;
using MVCtesting.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCtesting.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Order/MyOrders
        public async Task<IActionResult> MyOrders()
        {
            var customerId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems)           // Load OrderItems
                    .ThenInclude(oi => oi.Product)   // Load related Product info
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }


        // GET: Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Brand)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // POST: Order/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);

            var product = await _context.Tproducts.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var order = new Order
            {
                CustomerId = userId,
                OrderDate = System.DateTime.Now,
                Status = "Pending",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = quantity,
                        Price = product.Price ?? 0
                    }
                }
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("MyOrders");
        }

        // POST: Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var customerId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customerId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction(nameof(MyOrders));
            }

            if (order.Status == "Completed" || order.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "This order cannot be cancelled.";
                return RedirectToAction(nameof(MyOrders));
            }

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order cancelled successfully.";
            return RedirectToAction(nameof(MyOrders));
        }
    }
}
