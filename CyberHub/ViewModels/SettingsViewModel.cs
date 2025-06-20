using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CyberHub.ViewModels
{
    public class SettingsViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
        public string? ProfilePictureUrl { get; set; }


        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

       
    }
}
