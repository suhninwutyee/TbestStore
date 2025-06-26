using Microsoft.AspNetCore.Mvc;
using MVCtesting.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCtesting.Services;

namespace MVCtesting.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Payment/Pay/5
        public async Task<IActionResult> Pay(int id)
        {
            var customerId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customerId);

            if (order == null || order.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Order not found or already paid.";
                return RedirectToAction("MyOrders", "Order");
            }

            return View(order); // Return Payment/Pay.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int id, string method)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Invalid order.";
                return RedirectToAction("MyOrders", "Order");
            }

            order.PaymentMethod = method;
            order.Status = "Completed"; // Simulate payment success
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Payment successful using {method}.";
            return RedirectToAction("MyOrders", "Order");
        }


    }
}
