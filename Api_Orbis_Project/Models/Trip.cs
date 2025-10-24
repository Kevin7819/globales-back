using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api_Orbis_Project.Models
{
    public class Trip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TripId { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [MaxLength(255)]
        public string Origin { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Destination { get; set; } = string.Empty;

        [Required]
        public DateTime DepartureDate { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        [MaxLength(50)]
        public string FlightNumber { get; set; }

        [Required]
        public TripType Type { get; set; }
        
        [MaxLength(3)]
        public string CountryCode { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public bool IsUsed { get; set; } = false;
    }

}
