using Api_Orbis_Project.Dtos.Trip;
using Api_Orbis_Project.Models;
using System;

namespace Api_Orbis_Project.Mappers
{
    public static class TripMapper
    {
        public static TripDto ToDto(this Trip trip)
        {
            return new TripDto
            {
                TripId = trip.TripId,
                UserId = trip.UserId ?? 0, 
                Destination = trip.Destination,
                DepartureDate = trip.DepartureDate,
                ReturnDate = trip.ReturnDate,
                FlightNumber = trip.FlightNumber,
                Type = trip.Type.ToString()
            };
        }

        public static Trip ToTripFromCreateDto(this CreateTripRequestDto dto, int userId)
        {
            return new Trip
            {
                UserId = userId,               
                Destination = dto.Destination,
                DepartureDate = dto.DepartureDate,
                ReturnDate = dto.ReturnDate,
                FlightNumber = dto.FlightNumber ?? string.Empty,
                Type = Enum.Parse<TripType>(dto.Type, true),
                CountryCode = string.Empty, 
                ReservationCode = string.Empty, 
                IsUsed = false
            };
        }

        public static void UpdateTripFromDto(this Trip trip, UpdateTripRequestDto dto)
        {
            trip.Destination = dto.Destination;
            trip.DepartureDate = dto.DepartureDate;
            trip.ReturnDate = dto.ReturnDate;
            trip.FlightNumber = dto.FlightNumber ?? trip.FlightNumber;
            trip.Type = Enum.Parse<TripType>(dto.Type, true);
        }
    }
}