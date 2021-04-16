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

namespace Dashboard.Services.Intranet
{
    public class Notes
    {

        private Models.Dashboard.Intranet.Notes.Root root;

        public Notes(string IntranetAutologin)
        {
            HttpWebRequest apiRequest = WebRequest.Create("https://intra.epitech.eu/auth-" + IntranetAutologin + "/user/?format=json") as HttpWebRequest;

            string apiResponse = "";
            using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                apiResponse = reader.ReadToEnd();
            }

            root = (JsonConvert.DeserializeObject<Models.Dashboard.Intranet.Notes.Root>(apiResponse));
        }

        public Models.Dashboard.Intranet.Notes.Root getNotes()
        {
            return root;
        }

    }
}