using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class PaymentStatus
    {
        [Key]
        public int PaymentStatusId { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public ICollection<Payment>? Payments { get; set; }
    }
}