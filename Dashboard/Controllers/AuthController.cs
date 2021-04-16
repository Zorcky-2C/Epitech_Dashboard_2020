using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Dashboard.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace Dashboard.Controllers
{
    public class AuthController : Controller
    {

        private readonly ILogger<DashboardController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        [TempData]
        public string StatusMessage { get; set; }

        public AuthController(
            ILogger<DashboardController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GithubSignIn()
        {
            var redirectUri = new Uri(Url.Action("GithubLoginCallBack", "Auth", null, "http"));
            var githubClient = new OAuth2.Client.Impl.GitHubClient(new RequestFactory(), new OAuth2.Configuration.ClientConfiguration
            {
                ClientId = "9652875a42f2fa4910a3",
                ClientSecret = "28fdf93ed5d2b3973f0d98ecea36d3397e141583",
                Scope = "notifications",
                RedirectUri = redirectUri.ToString()
            });
            return Redirect(await githubClient.GetLoginLinkUriAsync());
        }

        public async Task<ActionResult> GithubLoginCallBack(string returnUrl = null, string remoteError = null)
        {
            var code = HttpContext.Request.Query["code"];

            var userInfo = new UserInfo();
            var redirectUri = new Uri(Url.Action("GithubLoginCallBack", "Auth", null, "http"));

            var githubClient = new OAuth2.Client.Impl.GitHubClient(new RequestFactory(), new OAuth2.Configuration.ClientConfiguration
            {
                ClientId = "9652875a42f2fa4910a3",
                ClientSecret = "28fdf93ed5d2b3973f0d98ecea36d3397e141583",
                Scope = "notifications",
                RedirectUri = redirectUri.ToString()
            });

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogDebug($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                user.GithubToken = await githubClient.GetTokenAsync(new NameValueCollection() { { "code", code } });

                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return LocalRedirect("/profile");
        }
    }
}
