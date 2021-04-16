using System;
using Microsoft.AspNetCore.Identity;

namespace Dashboard.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FullName { get; set; }

        [PersonalData]
        public string Services { get; set; }

        [PersonalData]
        public string Widgets { get; set; }

        [PersonalData]
        public string IntranetAutologin { get; set; }

        [PersonalData]
        public string GithubToken { get; set; }
    }
}
