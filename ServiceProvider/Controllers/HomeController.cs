using Microsoft.AspNetCore.Mvc;

namespace spWithIdpInitiated.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}