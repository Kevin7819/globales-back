using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api_Orbis_Project.Models
{
    public class Preference
    {
        [Key]
        public int PreferenceId { get; set; }

        // FK hacia User
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public string Category { get; set; }
        public string Value { get; set; }
    }
}
