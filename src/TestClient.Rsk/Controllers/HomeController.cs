using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestClient.Rsk.Models;

namespace TestClient.Rsk.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Login()
        {
            return Redirect("Index");
        }

        public ActionResult Logout()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
