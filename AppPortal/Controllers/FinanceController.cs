using AppPortal.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppPortal.Controllers
{
    public class FinanceController : Controller
    {
        private readonly FinanceContext _context;

        public FinanceController(FinanceContext context) => _context = context;

        public IActionResult Index()
        {
            return View();
        }
    }
}
