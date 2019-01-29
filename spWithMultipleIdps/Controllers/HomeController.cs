using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace spWithMultipleIdps.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Logout() => SignOut("cookie");
        public async Task<IActionResult> ChallengeScheme(string scheme)
        {
            var result = await HttpContext.AuthenticateAsync(scheme);
            if (result.Succeeded && result.Principal != null) return RedirectToAction("Index");

            return Challenge(scheme);
        }
    }
}