namespace SkyHigh.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        public string SeatNo { get; set; }
        public string Type { get; set; } // "VIP" or "Normal"
        public bool IsBooked { get; set; }
        public double Price { get; set; }
        public string Color { get; set; }
        public int FlightId { get; set; }
        public int? BookingId { get; set; }
    }
}

