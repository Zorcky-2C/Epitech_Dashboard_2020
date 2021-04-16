using System.ComponentModel.DataAnnotations;

namespace Dashboard.Models
{
    public class ProfileViewModel
    {
        [StringLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
        [Display(Name = "Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "UserName")]
        public string Username { get; set; }

        [Display(Name = "Services")]
        public string Services { get; set; }

        [Display(Name = "Widgets")]
        public string Widgets { get; set; }

        [Display(Name = "Widgets")]
        public string IntranetAutologin { get; set; }

        [Display(Name = "GithubToken")]
        public string GithubToken { get; set; }
    }
}
