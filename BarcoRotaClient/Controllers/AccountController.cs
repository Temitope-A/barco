using BarcoRota.Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BarcoRota.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly BarcoContext _context;

        public AccountController(BarcoContext context)
        {
            _context = context;
        }

        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("BarcoCierge", new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("BarcoCierge", new AuthenticationProperties
            {
                RedirectUri = Url.Action("signout-oidc", "Home")
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}