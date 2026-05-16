using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        public string RoomName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public int Capacity { get; set; }

        public ICollection<Session>? Sessions { get; set; }
    }
}