using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace spWithMultipleIdps.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public async Task<IActionResult> Logout()
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

                if (idp != null)
                {
                    await HttpContext.SignOutAsync();
                    var url = Url.Action("Logout");
                    return SignOut(new AuthenticationProperties {RedirectUri = url}, idp);
                }
            }

            return SignOut("cookie");
        }

        public async Task<IActionResult> ChallengeScheme(string scheme)
        {
            var result = await HttpContext.AuthenticateAsync(scheme);
            if (result.Succeeded && result.Principal != null)
            {
                var (user, provider, providerUserId, claims) = FindUserFromExternalProvider(result);

                await HttpContext.SignInAsync(user.SubjectId, user.Username, provider, claims.ToArray());
                await HttpContext.SignOutAsync("cookie");
                return RedirectToAction("Index");
            }
            
            return Challenge(scheme);
        }

        private (TestUser user, string provider, string providerUserId, IEnumerable<Claim> claims) FindUserFromExternalProvider(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items[".AuthScheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            
            return (new TestUser(){SubjectId = providerUserId}, provider, providerUserId, claims);
        }

    }
}