using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class SetupMFAViewModel
    {
        [Display(Name = "Security Key")]
        public string? SecurityKey { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; } = string.Empty;

        public byte[]? QRCode { get; set; }
    }
}
