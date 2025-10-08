namespace Api_Orbis_Project.Dtos.Trip
{
    public class TripDto
    {
        public int TripId { get; set; }
        public int UserId { get; set; }  
        public string Destination { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? FlightNumber { get; set; }
        public string Type { get; set; }
    }

    public class CreateTripRequestDto
    {
        public string Destination { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? FlightNumber { get; set; }
        public string Type { get; set; } 
    }

    public class UpdateTripRequestDto
    {
        public string Destination { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? FlightNumber { get; set; }
        public string Type { get; set; } 
    }
}
