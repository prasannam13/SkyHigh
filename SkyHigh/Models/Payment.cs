using System;

namespace SkyHigh.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public double Amount { get; set; }
        public double GST { get; set; }
        public double GstRate { get; set; }
        public double TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
       
        public Booking Booking { get; set; }
    }
}

