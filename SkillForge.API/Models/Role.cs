using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
