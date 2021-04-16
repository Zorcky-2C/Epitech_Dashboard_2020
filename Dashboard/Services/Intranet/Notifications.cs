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
using Dashboard.Models.Dashboard;
using System.Text.RegularExpressions;

namespace Dashboard.Services.Intranet
{
    public class Notifications
    {
        private List<Models.Dashboard.Intranet.Notifications.Notification> root;

        public Notifications(string IntranetAutologin)
        {
            HttpWebRequest apiRequest = WebRequest.Create("https://intra.epitech.eu/auth-" + IntranetAutologin + "/user/notification/message?format=json") as HttpWebRequest;

            string apiResponse = "";
            using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                apiResponse = reader.ReadToEnd();
            }

            root = (JsonConvert.DeserializeObject<List<Models.Dashboard.Intranet.Notifications.Notification>>(apiResponse));
        }

        public List<Models.Dashboard.Intranet.Notifications.Notification> getNotifications()
        {
            return root;
        }

        public String getCleanString(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}