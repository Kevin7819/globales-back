using System;
using System.ComponentModel.DataAnnotations;

namespace Api_Orbis_Project.Models
{
    public class DeviceRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string ExpoPushToken { get; set; }

        [MaxLength(20)]
        public string? Platform { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}