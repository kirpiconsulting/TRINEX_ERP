using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace erpv01.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Zaten giriş yapmışsa Anasayfaya at
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 1. AppSettings'den kullanıcıları çek
            var users = _configuration.GetSection("AppUsers").Get<List<AppUser>>();

            // 2. Kullanıcıyı bul
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // 3. Kimlik Kartı (Claims) Oluştur
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("DisplayName", user.DisplayName) // Ekranda adını göstermek için
                };

                var claimsIdentity = new ClaimsIdentity(claims, "TrinexCookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Tarayıcı kapansa da hatırla
                };

                // 4. Çerezi oluştur ve giriş yap
                await HttpContext.SignInAsync("TrinexCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                return Json(new { success = true, redirectUrl = "/Home/Index" });
            }

            return Json(new { success = false, message = "Kullanıcı adı veya parola hatalı!" });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("TrinexCookieAuth");
            return RedirectToAction("Login");
        }
    }

    // AppSettings verisini tutacak basit sınıf
    public class AppUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string DisplayName { get; set; }
    }
}