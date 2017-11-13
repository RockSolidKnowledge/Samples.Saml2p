using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace TestClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                Request.GetOwinContext()
                    .Authentication.Challenge(new AuthenticationProperties { RedirectUri = Url.Action("Login") }, "saml2p");
                return new HttpUnauthorizedResult();
            }

            return Redirect("Index");
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();

            return Redirect("Index");
        }
    }
}