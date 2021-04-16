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

namespace Dashboard.Services.CatAPI
{
    public class Cat
    {

        private List<Models.Dashboard.CatAPI.Cat.CatJson> root;

        public Cat()
        {
            var json = new WebClient().DownloadString("https://api.thecatapi.com/v1/images/search?size=medium");

            root = (JsonConvert.DeserializeObject<List<Models.Dashboard.CatAPI.Cat.CatJson>>(json));
        }

        public List<Models.Dashboard.CatAPI.Cat.CatJson> getData()
        {
            return root;
        }

    }
}
