using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class UserProfileViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

       
        [Required(ErrorMessage = "Department field is required")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Position field is required")]
        public string Position { get; set; } = string.Empty;

        [Display(Name = "Do you want to enable 2FA ?")]
        public bool EnableTwoFactorAuth { get; set; }

    }
}
