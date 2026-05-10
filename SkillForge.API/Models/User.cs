using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillForge.API.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
    }
}
