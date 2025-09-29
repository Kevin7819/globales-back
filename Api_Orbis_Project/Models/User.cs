using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Xml;

namespace Api_Orbis_Project.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CountryOfOrigin { get; set; }
        public string? PreferredLanguage { get; set; }
        public DateOnly? BirthDate { get; set; }

        public Role UserRole { get; set; }

        // Navigation properties
        public ICollection<Trip> Trips { get; set; }
        public ICollection<Preference> Preferences { get; set; }
        public ICollection<Alert> Alerts { get; set; }
        public ICollection<ChatHistory> ChatHistories { get; set; }

        //public static implicit operator User(User v)
        //{
         //   throw new NotImplementedException();
       // }

        public enum Role
        {
            Passenger,
            Admin
        }
    }
}
