using System.ComponentModel.DataAnnotations;

namespace SkyHigh.Models
{
    public class AadharViewModel
    {
        [Required]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Aadhaar number is required.")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhaar must be exactly 12 digits.")]
        [Display(Name = "Aadhaar Number")]
        public string AadharNumber { get; set; } = string.Empty;
    }
}