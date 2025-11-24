using Microsoft.AspNetCore.Mvc;

namespace erpv01.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
