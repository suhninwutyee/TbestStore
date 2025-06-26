using Microsoft.AspNetCore.Mvc;

namespace MVCtesting.Controllers
{
    public class SupportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitSupport(string Subject, string Message)
        {
            // Optional: save or send message
            TempData["SupportMessage"] = "Your support request has been received.";
            return RedirectToAction("Index");
        }
    }
}
