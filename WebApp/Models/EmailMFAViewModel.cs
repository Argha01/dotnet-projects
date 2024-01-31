using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class EmailMFAViewModel
    {
        [Required(ErrorMessage = "Please provide the Auth Code!!!")]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
