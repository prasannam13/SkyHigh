
using Microsoft.AspNetCore.Mvc;
using SkyHigh.Models;
using System.Collections.Generic;
using System.Linq;

namespace SkyHigh.Controllers
{
    public class OffersController : Controller
    {
        private static readonly List<Offer> Offers = BuildOffers();

        public IActionResult Index()
        {
            return View(Offers);
        }

        public IActionResult Details(int id)
        {
            var offer = Offers.FirstOrDefault(o => o.Id == id);
            if (offer == null) return RedirectToAction("Index");
            return View(offer);
        }

        // Build all demo offers
        private static List<Offer> BuildOffers()
        {
            var list = new List<Offer>
            {

                new Offer
                {
                    Id = 1,
                    Title = "Diwali Delight",
                    Category = "Festive",
                    ExpiresOn = "10 Nov 2025",
                    DiscountText = "Up to ₹1,500 Off",
                    ShortDesc = "Celebrate Diwali with amazing savings on domestic flights.",
                    ImageUrl = "/images/offers/diwali.png",
                    SalePeriod = "Now - 10 Nov 2025",
                    TravelPeriod = "Nov 2025 - Feb 2026",
                    Details = new List<string> {
                        "Valid on select domestic routes only.",
                        "Book during the sale period to avail discounts.",
                        "Offer not combinable with other ongoing promotions.",
                        "Limited seats available, terms and conditions apply."
                    }
                },

                new Offer
                {
                Id = 2,
                Title = "Holi Savings",
                Category = "Festive",
                ExpiresOn = "31 Mar 2026",
                DiscountText = "Flat 20% Off",
                ShortDesc = "Celebrate the colors of Holi with exclusive flight discounts.",
                ImageUrl = "/images/offers/holi.png",
                SalePeriod = "Now - 31 Mar 2026",
                TravelPeriod = "Mar - May 2026",
                Details = new List<string> {
                    "Applicable on select flights across major domestic destinations.",
                    "Tickets are subject to availability.",
                    "Offer valid only during the Holi festive period.",
                    "T&C apply on rescheduling and cancellation."
                    }
                },

                new Offer
                {
                    Id = 3,
                    Title = "Kotak Credit Offer",
                    Category = "Bank",
                    ExpiresOn = "31 Dec 2025",
                    DiscountText = "10% + EMI",
                    ShortDesc = "Pay with Kotak Cards and get instant discount + EMI.",
                    ImageUrl = "/images/offers/kotak.png",
                    SalePeriod = "Now - 31 Dec 2025",
                    TravelPeriod = "Jan - Jun 2026",
                    Details = new List<string> {
                        "Valid only on Kotak Bank credit cards.",
                        "EMI options available as per bank terms."
                    }
                },

                new Offer
                {
                    Id = 4,
                    Title = "J&K Bank Offer",
                    Category = "Bank",
                    ExpiresOn = "31 Dec 2025",
                    DiscountText = "Flat 15% Off",
                    ShortDesc = "Use J&K Bank Card for instant savings on your next booking.",
                    ImageUrl = "/images/offers/jkbank.png",
                    SalePeriod = "Now - 31 Dec 2025",
                    TravelPeriod = "Jan - Jun 2026",
                    Details = new List<string> {
                        "Offer applicable only on payments made using J&K Bank Credit or Debit Cards.",
                        "Discount available on both one-way and round-trip domestic flights.",
                        "Offer cannot be combined with other bank or festive discounts.",
                        "Valid for bookings made directly on SkyHigh’s website."
                    }
                },


                new Offer
                {
                    Id = 5,
                    Title = "Monsoon Saver",
                    Category = "Seasonal",
                    ExpiresOn = "30 Sep 2025",
                    DiscountText = "Flat 50% Off",
                    ShortDesc = "Special monsoon fares for selected domestic routes.",
                    ImageUrl = "/images/offers/monsoon.png",
                    SalePeriod = "Now - 30 Sep 2025",
                    TravelPeriod = "Now - 31 Oct 2025",
                    Details = new List<string> {
                        "Applicable on select domestic sectors.",
                        "Non combinable with other promotions or vouchers.",
                        "Limited seats per flight under the promotional fare.",
                        "Tickets once booked under offer are non-transferable."
                    }
                },


                new Offer
                {
                    Id = 6,
                    Title = "HDFC Cashback",
                    Category = "Bank",
                    ExpiresOn = "31 Dec 2025",
                    DiscountText = "₹1,000 Cashback",
                    ShortDesc = "Get up to ₹1,000 cashback when you pay using your HDFC Bank card.",
                    ImageUrl = "/images/offers/hdfc.png",
                    SalePeriod = "Now - 31 Dec 2025",
                    TravelPeriod = "Jan - Jun 2026",
                    Details = new List<string> {
                        "Offer valid only on HDFC Bank Credit and Debit Cards.",
                        "Cashback credited within 7 working days after travel date.",
                        "Offer not applicable with other bank promotions.",
                        "Bookings must be made directly on SkyHigh’s website."
                    }
                },

                new Offer
                {
                    Id = 7,
                    Title = "Diwali AirFest",
                    Category = "Festive",
                    ExpiresOn = "10 Nov 2025",
                    DiscountText = "Up to 30% Off",
                    ShortDesc = "Celebrate Diwali with SkyHigh’s special AirFest discounts on key routes.",
                    ImageUrl = "/images/offers/diwaliairfest.png",
                    SalePeriod = "Now - 10 Nov 2025",
                    TravelPeriod = "Nov 2025 - Feb 2026",
                    Details = new List<string> {
                        "Applicable on select domestic routes.",
                        "Offer available for both one-way and round-trip bookings.",
                        "Discounts vary based on destination and travel dates.",
                        "Limited availability during peak festive season."
                    }
                },

                new Offer
                {
                    Id = 8,
                    Title = "Daily Flash",
                    Category = "DayDeal",
                    ExpiresOn = "Ongoing (Daily)",
                    DiscountText = "Limited-time",
                    ShortDesc = "Grab exclusive daily deals — lowest fares available for 24 hours only.",
                    ImageUrl = "/images/offers/dailyflash.png",
                    SalePeriod = "Every 24 hours - new deals daily",
                    TravelPeriod = "Next 30 days",
                    Details = new List<string> {
                        "Applicable on domestic flights with select airlines.",
                        "Offer valid for 24 hours only from time of release.",
                        "Limited seats per route under flash fare.",
                        "Non-refundable and non-changeable after booking."
                    }
                },


                new Offer
                {
                    Id = 9,
                    Title = "AirMiles Double",
                    Category = "Reward",
                    ExpiresOn = "31 Mar 2026",
                    DiscountText = "Earn 2x Miles",
                    ShortDesc = "Earn double SkyHigh AirMiles on every booking during the promotional period.",
                    ImageUrl = "/images/offers/airmiles.png",
                    SalePeriod = "Now - 31 Mar 2026",
                    TravelPeriod = "Now - 31 May 2026",
                    Details = new List<string> {
                        "Members must log in to their SkyHigh account before booking.",
                        "Bonus miles credited within 7 days after travel completion.",
                        "Not applicable for infant tickets or award redemptions.",
                        "Offer valid only on eligible flight categories."
                    }
                },

                new Offer
                {
                    Id = 10,
                    Title = "Independence Sale",
                    Category = "Festive",
                    ExpiresOn = "20 Aug 2025",
                    DiscountText = "Up to 50% Off",
                    ShortDesc = "Celebrate Independence Day with massive discounts on domestic flights.",
                    ImageUrl = "/images/offers/independence.png",
                    SalePeriod = "10 Aug - 20 Aug 2025",
                    TravelPeriod = "Aug - Oct 2025",
                    Details = new List<string> {
                        "Offer valid for limited time only during Independence Week.",
                        "Discounts vary based on route and travel date.",
                        "Applicable only on direct domestic flights.",
                        "Tickets booked under this sale are non-refundable."
                    }
                },

                new Offer
                {
                    Id = 11,
                    Title = "Goa Summer Special",
                    Category = "Seasonal",
                    ExpiresOn = "30 Apr 2026",
                    DiscountText = "Up to ₹1200 off",
                    ShortDesc = "Summer fares for Goa getaways.",
                    ImageUrl = "/images/offers/goa.png",
                    SalePeriod = "Now - 30 Apr 2026",
                    TravelPeriod = "Mar - Jun 2026",
                    Details = new List<string> {
                        "Limited seats per flight.",
                        "Non-refundable on cheapest fares."
                    }
                },

                new Offer
                {
                    Id = 12,
                    Title = "Student Offer",
                    Category = "Special",
                    ExpiresOn = "31 Dec 2025",
                    DiscountText = "Extra 15kg Baggage",
                    ShortDesc = "Exclusive benefits for students traveling with SkyHigh.",
                    ImageUrl = "/images/offers/student.png",
                    SalePeriod = "Now - 31 Dec 2025",
                    TravelPeriod = "Jan - Jun 2026",
                    Details = new List<string> {
                        "Offer valid only for students with valid student ID proof.",
                        "Additional 15kg check-in baggage allowance.",
                        "Discount applicable only on base fare.",
                        "Not valid for group bookings or infant tickets."
                    }
                },

                new Offer
                {
                    Id = 13,
                    Title = "Women’s Day Offer",
                    Category = "Festive",
                    ExpiresOn = "10 Mar 2026",
                    DiscountText = "25% Off",
                    ShortDesc = "Celebrate Women’s Day with 25% off on domestic flights.",
                    ImageUrl = "/images/offers/womensday.png",
                    SalePeriod = "1 Mar - 10 Mar 2026",
                    TravelPeriod = "Mar - May 2026",
                    Details = new List<string> {
                        "Offer valid only for female passengers.",
                        "Applicable on one-way and round-trip bookings.",
                        "Seats subject to availability.",
                        "T&C apply; non-transferable and non-refundable."
                    }
                },


                new Offer
                {
                    Id = 14,
                    Title = "Corporate Offer",
                    Category = "Business",
                    ExpiresOn = "31 Dec 2025",
                    DiscountText = "Flat ₹500 Cashback",
                    ShortDesc = "Book business trips with SkyHigh and get ₹500 instant cashback.",
                    ImageUrl = "/images/offers/corporate.png",
                    SalePeriod = "Now - 31 Dec 2025",
                    TravelPeriod = "Jan - Jun 2026",
                    Details = new List<string> {
                        "Applicable for corporate and business travelers only.",
                        "Booking must be done with registered corporate email ID.",
                        "Cashback processed within 7 working days post-travel.",
                        "Not valid with other cashback or bank offers."
                    }
                },


                new Offer
                {
                    Id = 15,
                    Title = "Family Pack",
                    Category = "Festive",
                    ExpiresOn = "25 Dec 2025",
                    DiscountText = "Book 3 Get 1 Free",
                    ShortDesc = "Enjoy family travel with one free ticket for every three bookings.",
                    ImageUrl = "/images/offers/family.png",
                    SalePeriod = "Now - 25 Dec 2025",
                    TravelPeriod = "Dec 2025 - Mar 2026",
                    Details = new List<string> {
                        "Offer applicable for bookings with 4 or more passengers.",
                        "The lowest fare among the 4 tickets will be waived.",
                        "Only valid for domestic flights.",
                        "Limited availability; blackout dates apply."
                    }
                }

            };

            return list;
        }
    }
}


