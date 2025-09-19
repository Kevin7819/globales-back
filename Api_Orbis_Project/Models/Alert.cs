using System;
using System.ComponentModel.DataAnnotations;

namespace Api_Orbis_Project.Models
{
    public class Alert
    {
        [Key]
        public int AlertId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string? Message { get; set; }
        public DateTime SentAt { get; set; }

        public AlertType Type { get; set; }

        public enum AlertType
        {
            Health,
            Culture,
            Safety
        }
    }
}
