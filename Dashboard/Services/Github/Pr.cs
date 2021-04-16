using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Dashboard.Models.Identity;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using Octokit;
using Octokit.Internal;
using RestSharp;
using System.Runtime.Serialization.Json;

namespace Dashboard.Services.Github
{

    public class Pr
    {

        public IReadOnlyList<Notification> Notifications { get; set; }

        public Pr(string githubcookie)
        {
            Task.Run(() => this.getNotifications(githubcookie)).Wait();
        }

        public async Task getNotifications(string githubcookie)
        {
            var github = new GitHubClient(new ProductHeaderValue("Dashboard"), new InMemoryCredentialStore(new Credentials(githubcookie)));
            var options = new ApiOptions { PageSize = 5, PageCount = 1 };
            Notifications = await github.Activity.Notifications.GetAllForCurrent(options);
        }

    }
}
