using Microsoft.AspNetCore.Mvc;
using MVCtesting.Services;

namespace MVCtesting.Controllers
{
    public class AuditController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var logs = _context.AuditLogs.OrderByDescending(l => l.Timestamp).ToList();
            return View(logs);
        }
       
    }
}
