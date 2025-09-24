using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api_Orbis_Project.Models
{
    public class Trip
    {
        [Key]
        public int TripId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string? Destination { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? FlightNumber { get; set; }

        public TripType Type { get; set; }

        public ICollection<Guide> Guides { get; set; }

        public enum TripType
        {
            Tourism,
            Business,
            Health
        }
    }
}
