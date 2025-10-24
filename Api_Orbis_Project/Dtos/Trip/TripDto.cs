namespace Api_Orbis_Project.Dtos.Trip
{
    public class TripDto
    {
        public int TripId { get; set; }
        public int UserId { get; set; }  
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? FlightNumber { get; set; }
        public string Type { get; set; } = string.Empty;
        public string ReservationCode { get; set; } = string.Empty;
    }

    public class CreateTripRequestDto
    {
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? FlightNumber { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class UpdateTripRequestDto
    {
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? FlightNumber { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class ClaimTripRequest
    {
        public string ReservationCode { get; set; } = string.Empty;
    }
}