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

namespace Dashboard.Services.Ip
{
    public class Ip
    {

        private Models.Dashboard.Ip.IpGrab.Root ipGetter;
        private Models.Dashboard.Ip.IpInfo.Root root;

        public Ip()
        {
            var json = new WebClient().DownloadString("https://api.ipify.org/?format=json");

            ipGetter = (JsonConvert.DeserializeObject<Models.Dashboard.Ip.IpGrab.Root>(json));
        }

        public Models.Dashboard.Ip.IpInfo.Root getIpInfo()
        {
            var json = new WebClient().DownloadString("https://ipinfo.io/" + ipGetter.Ip + "/geo");
        
            root = (JsonConvert.DeserializeObject<Models.Dashboard.Ip.IpInfo.Root>(json));
            return root;
        }

    }
}
