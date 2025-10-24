using System;
using System.ComponentModel.DataAnnotations;

namespace Api_Orbis_Project.Models
{
    public class DeviceRegistration
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;

        [Required]
        public string ExpoPushToken { get; set; } = default!;

        [Required]
        public string Platform { get; set; } = "android";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}