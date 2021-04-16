using System.Diagnostics;
using System.Threading.Tasks;
using Dashboard.Infrastructure;
using Dashboard.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Dashboard.Models;
using Dashboard.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Collections.Generic;
using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Net;
using System.IO;

namespace Dashboard.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        [TempData]
        public string StatusMessage { get; set; }

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("/")]
        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View(new ProfileViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Services = user.Services,
                Widgets = user.Widgets,
                IntranetAutologin = user.IntranetAutologin,
                GithubToken = user.GithubToken
            });
        }

        [HttpGet("/architecture")]
        public IActionResult Architecture()
        {
            return View();
        }

        [ImportModelState]
        [HttpGet("/profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View(new ProfileViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Services = user.Services,
                Widgets = user.Widgets,
                IntranetAutologin = user.IntranetAutologin,
                GithubToken = user.GithubToken
            });
        }

        [ExportModelState]
        [HttpPost("/profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromForm]
            ProfileViewModel input)
        {
            if (!ModelState.IsValid)
            { 
                return RedirectToAction(nameof(Profile));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Model state might not be valid anymore if we weren't able to change the e-mail address
            // so we need to check for that before proceeding
            if (ModelState.IsValid)
            {
                if (input.FullName != user.FullName)
                {
                    // If we receive an empty string, set a null full name instead
                    user.FullName = string.IsNullOrWhiteSpace(input.FullName) ? null : input.FullName;
                }

                await _userManager.UpdateAsync(user);

                await _signInManager.RefreshSignInAsync(user);

                StatusMessage = "Your profile has been updated";
            }

            return RedirectToAction(nameof(Profile));
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(IndexAsync));
            }
        }

        [HttpGet("/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("/status-code")]
        public IActionResult StatusCodeHandler(int code)
        {
            ViewBag.StatusCode = code;
            ViewBag.StatusCodeDescription = ReasonPhrases.GetReasonPhrase(code);
            ViewBag.OriginalUrl = null;


            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusCodeReExecuteFeature != null)
            {
                ViewBag.OriginalUrl =
                    statusCodeReExecuteFeature.OriginalPathBase
                    + statusCodeReExecuteFeature.OriginalPath
                    + statusCodeReExecuteFeature.OriginalQueryString;
            }

            if (code == 404)
            {
                return View("Status404");
            }

            return View("Status4xx");
        }

        [HttpGet("/about.json")]
        public string Get()
        {

            Json json = new Json();
            json.Client = new JsonClient();
            json.Client.Host = GetPublicIP();

            json.Server = new JsonServer();
            json.Server.CurrentTime = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            json.Server.Services = new List<JsonService>();

            foreach (var Service in Startup.Services)
            {
                JsonService service = new JsonService();
                service.Name = Service.Name;
                service.Widgets = new List<JsonWidget>();

                json.Server.Services.Add(service);

                foreach (var Widget in Service.Widgets)
                {
                    JsonWidget widget = new JsonWidget();
                    widget.Name = Widget.Name;
                    widget.Description = Widget.Description;
                    widget.Params = new List<JsonParam>();

                    service.Widgets.Add(widget);
                }
            }
            return JsonConvert.SerializeObject(json, Formatting.Indented);
        }
        public static string GetPublicIP()
        {
            string myPublicIp = "";
            WebRequest request = WebRequest.Create("https://api.ipify.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                myPublicIp = stream.ReadToEnd();
            }
            return myPublicIp;
        }
    }
}
