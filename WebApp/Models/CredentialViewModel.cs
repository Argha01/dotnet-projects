using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class CredentialViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Invalid Password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
