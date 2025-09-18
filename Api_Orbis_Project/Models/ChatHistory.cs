using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api_Orbis_Project.Models
{
    public class ChatHistory
    {
        [Key]
        public int ChatId { get; set; }

        // FK hacia User
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public string Question { get; set; }
        public string Answer { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
