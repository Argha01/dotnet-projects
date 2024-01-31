using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class RegisterUserViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Invalid Password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department field is required")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Position field is required")]
        public string Position { get; set; } = string.Empty;

    }
}
