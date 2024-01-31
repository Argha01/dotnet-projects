using System.ComponentModel.DataAnnotations;

namespace WebApp_UnderTheHood.Models
{
    public class CredentialViewModel
    {
        [Required(ErrorMessage = "User Name cannot be blank")]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password cannot be blank")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]   
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; } 
    }
}
