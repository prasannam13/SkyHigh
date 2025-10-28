using System.Collections.Generic;

namespace SkyHigh.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Category { get; set; } = ""; // e.g. Seasonal, Bank, Festive, DayDeal
        public string ExpiresOn { get; set; } = "";
        public string DiscountText { get; set; } = "";
        public string ShortDesc { get; set; } = "";
        public string ImageUrl { get; set; } = ""; // data:image/svg+xml... or normal url
        public string SalePeriod { get; set; } = "";
        public string TravelPeriod { get; set; } = "";
        public List<string> Details { get; set; } = new();
    }
}
