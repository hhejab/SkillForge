using System.ComponentModel.DataAnnotations;

namespace SkillForge.MVC.Models
{
    public class CardPaymentViewModel
    {
        public int PaymentId { get; set; }

        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Name on card is required.")]
        public string CardholderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Card number is required.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be 16 digits.")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiry month is required.")]
        [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12.")]
        public int ExpiryMonth { get; set; }

        [Required(ErrorMessage = "Expiry year is required.")]
        [Range(2026, 2100, ErrorMessage = "Expiry year must be current or future year.")]
        public int ExpiryYear { get; set; }

        [Required(ErrorMessage = "CVV is required.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be 3 digits.")]
        public string CVV { get; set; } = string.Empty;
    }
}

