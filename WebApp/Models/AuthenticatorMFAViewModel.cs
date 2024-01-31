using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class AuthenticatorMFAViewModel
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
