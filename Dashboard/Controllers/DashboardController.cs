using Dashboard.Infrastructure;
using Dashboard.Models;
using Dashboard.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Controllers
{
    public class DashboardController : Controller
    {

        private readonly ILogger<DashboardController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        [TempData]
        public string StatusMessage { get; set; }

        public DashboardController(
            ILogger<DashboardController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task RemoveWidgetAsync(string servicename, string widgetname)
        {
            await DeleteWidgetFromDbAsync(widgetname);
        }

        public async Task AddWidgetAsync(string servicename, string widgetname)
        {
            string name = Startup.GetService(servicename).GetWidget(widgetname).Id;
            await AddWidgetToDbAsync(name);
        }

        public async Task<ActionResult> AddServiceAsync(string servicename)
        {
            await AddServiceToDbAsync(servicename);
            if (servicename == "Github")
            {
                return RedirectToAction("GithubSignIn", "Auth");
            }
            return LocalRedirect("/profile");
        }

        public async Task DeleteServiceAsync(string servicename)
        {
            await DeleteServiceFromDbAsync(servicename);
        }

        public async Task DeleteServiceFromDbAsync(string servicename)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogDebug($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            List<string> Items = user.Services.Split(':').Select(i => i.Trim()).Where(i => i != string.Empty).ToList();
            Items.Remove(servicename);

            user.Services = "";

            foreach (string service in Items)
            {
                user.Services += service + ":";
            }

            await _userManager.UpdateAsync(user);
        }

        public async Task AddServiceToDbAsync(string servicename)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogDebug($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (user.Services.Contains(servicename) == false)
            {
                user.Services += servicename + ":";
            }

            await _userManager.UpdateAsync(user);
        }

        public async Task DeleteWidgetFromDbAsync(string widgetname)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogDebug($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            List<string> Items = user.Widgets.Split(':').Select(i => i.Trim()).Where(i => i != string.Empty).ToList();

            foreach (var item in Items)
            {
                if (item.StartsWith(widgetname))
                {
                    Items.Remove(item);
                    break;
                }
            }

            user.Widgets = "";

            foreach (string widget in Items)
            {
                user.Widgets += widget + ":";
            }

            await _userManager.UpdateAsync(user);
        }

        public async Task<IActionResult> SaveWidgetPosAsync(string widgetid, string x, string y)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogDebug($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            }

            List<string> Items = user.Widgets.Split(':').Select(i => i.Trim()).Where(i => i != string.Empty).ToList();

            user.Widgets = "";

            foreach (string widget in Items)
            {
                if (widget.Contains(widgetid))
                {
                    user.Widgets += ChangeWidgetPos(widget, x, y) + ":";
                }
                else
                {
                    user.Widgets += widget + ":";
                }
            }

            await _userManager.UpdateAsync(user);
            return View();
        }

        private string ChangeWidgetPos(string widget, string x, string y)
        {
            x = x.Substring(0, x.Length - 2);
            y = y.Substring(0, y.Length - 2);
            return widget.Split('=')[0] + "=" + x + "," + y;
        }

        public async Task AddWidgetToDbAsync(string widgetname)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogDebug($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (user.Widgets.Contains(widgetname) == false)
            {
                user.Widgets += widgetname + "=0,0" + ":";
            }

            await _userManager.UpdateAsync(user);
        }

        public async Task<ActionResult> SaveIntranetAutologinAsync(string autologin)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogDebug($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            user.IntranetAutologin = autologin;

            await _userManager.UpdateAsync(user);

            await AddServiceAsync("Epitech Intranet");

            return LocalRedirect("/profile");
        }
    }
}
