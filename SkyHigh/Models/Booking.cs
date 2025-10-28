

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkyHigh.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        [Required]
        public int FlightId { get; set; }
        public int? ReturnFlightId { get; set; }
        [Required]
        [StringLength(20)]
        public string PNR { get; set; }
        [StringLength(50)]
        public string Class { get; set; }
        public string SeatsBooked { get; set; }
        public DateTime BookingDate { get; set; }
        [Range(0,double.MaxValue)]
        public double TotalAmount { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public string TripType { get; set; } = "OneWay";
        public string? BookingType { get; set; }

    
        public Flight? Flight { get; set; }
        public Flight? ReturnFlight { get; set; }

     
        public List<Passenger> Passengers { get; set; } = new();
        public Payment? Payment { get; set; }
        public string AadharNumber {  get; set; }
        public bool AadharVerified {  get; set; }
    }
}

