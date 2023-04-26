using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace spWithSingleIdp.Controllers;

public class HomeController : Controller
{
    [Route("/")]
    public ViewResult Index()
    {
        return View();
    }

    [Route("/sign-in")]
    [Authorize]
    public IActionResult SignIn()
    {
        return Redirect("/");
    }

    [Route("/sign-out")]
    public IActionResult FullLogout()
    {
        // Sign out of the application session ( cookie )
        // Sign out of the saml scheme, this will cause a redirect to SAML IDP to sign out
        return SignOut( "cookie", "saml");
    }
    
    [Route("/app-sign-out")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("cookie");
        return Redirect("/");
    }
}