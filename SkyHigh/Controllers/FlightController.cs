
using Microsoft.AspNetCore.Mvc;
using SkyHigh.Data;
using SkyHigh.Helpers;
using SkyHigh.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkyHigh.Controllers
{
    public class FlightController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FlightController(ApplicationDbContext context) => _context = context;

        public IActionResult Search() => View();

        // Accept optional TripType and ReturnDate so view can pass them (backwards compatible)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(string source, string destination, DateTime departureDate, string TripType = "OneWay", DateTime? ReturnDate = null)
        {
            var flights = _context.Flights.Where(f =>
                f.Source == source &&
                f.Destination == destination &&
                f.DepartureTime.Date == departureDate.Date).ToList();

            // forward trip type and return date so SearchResults view can continue the flow
            ViewBag.TripType = TripType ?? "OneWay";
            ViewBag.ReturnDate = ReturnDate;

            return View("SearchResults", flights);
        }

        // Admin: Add Flight
        public IActionResult AddFlight()
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return Unauthorized();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFlight(Flight flight)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return Unauthorized();

            // Ensures the value from dropdown is parsed correctly
            if (Request.Form.ContainsKey("HasBusinessClass"))
            {
                var val = Request.Form["HasBusinessClass"];
                flight.HasBusinessClass = val == "true";
            }

            if (ModelState.IsValid)
            {
                _context.Flights.Add(flight);
                _context.SaveChanges();

                var seats = GenerateSeats(flight.FlightId, flight.EconomyFare, flight.BusinessFare, flight.HasBusinessClass);

                foreach (var seat in seats)
                {
                    seat.FlightId = flight.FlightId;
                    _context.Seats.Add(seat);
                }
                _context.SaveChanges();
                return RedirectToAction("ViewFlights");
            }
            return View(flight);
        }

        // Admin: Edit Flight
        public IActionResult EditFlight(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return Unauthorized();
            var flight = _context.Flights.Find(id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditFlight(Flight flight)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return Unauthorized();

            if (Request.Form.ContainsKey("HasBusinessClass"))
            {
                var val = Request.Form["HasBusinessClass"];
                flight.HasBusinessClass = val == "true";
            }

            var oldFlight = _context.Flights.Find(flight.FlightId);
            if (oldFlight == null) return NotFound();

            // Update properties
            oldFlight.FlightNo = flight.FlightNo;
            oldFlight.Source = flight.Source;
            oldFlight.Destination = flight.Destination;
            oldFlight.DepartureTime = flight.DepartureTime;
            oldFlight.ArrivalTime = flight.ArrivalTime;
            oldFlight.Duration = flight.Duration;
            oldFlight.EconomyFare = flight.EconomyFare;
            oldFlight.BusinessFare = flight.BusinessFare;
            oldFlight.HasBusinessClass = flight.HasBusinessClass;
            _context.SaveChanges();

            // Delete old seats
            var oldSeats = _context.Seats.Where(s => s.FlightId == flight.FlightId).ToList();
            _context.Seats.RemoveRange(oldSeats);
            _context.SaveChanges();

            // Generate and add new seats
            var seats = GenerateSeats(flight.FlightId, flight.EconomyFare, flight.BusinessFare, flight.HasBusinessClass);
            foreach (var seat in seats)
            {
                seat.FlightId = flight.FlightId;
                _context.Seats.Add(seat);
            }
            _context.SaveChanges();

            return RedirectToAction("ViewFlights");
        }


        // Admin: View Flights
        public IActionResult ViewFlights()
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return Unauthorized();
            var flights = _context.Flights.ToList();
            return View(flights);
        }

        // Admin: Delete Flight
        public IActionResult DeleteFlight(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return Unauthorized();
            var flight = _context.Flights.Find(id);
            if (flight != null)
            {
                var seats = _context.Seats.Where(s => s.FlightId == id).ToList();
                _context.Seats.RemoveRange(seats);
                _context.Flights.Remove(flight);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewFlights");
        }

        // Admin: View Seats for a Flight
        public IActionResult ViewSeats(int flightId)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return Unauthorized();
            var seats = _context.Seats.Where(s => s.FlightId == flightId).ToList();
            return View(seats);
        }

        private List<Seat> GenerateSeats(int flightId, double economyFare, double businessFare, bool hasBusinessClass)
        {
            var seats = new List<Seat>();
            int totalSeats = 30;
            int businessSeats = hasBusinessClass ? 10 : 0; // first 10 seats (2 rows of 5)
            string[] columns = { "A", "B", "C", "D", "E" }; // 5 seats per row

            int seatNumber = 1;

            // Business class seats (if enabled)
            if (hasBusinessClass)
            {
                for (int row = 1; row <= 2; row++)
                {
                    foreach (var col in columns)
                    {
                        bool isWindow = (col == "A" || col == "E");
                        seats.Add(new Seat
                        {
                            FlightId = flightId,
                            SeatNo = $"{row}{col}",
                            Type = "VIP",
                            Price = isWindow ? businessFare * 1.3 : businessFare,
                            Color = isWindow ? "#FFF789" : "#FFD700",
                            IsBooked = false
                        });
                        seatNumber++;
                    }
                }
            }

            // Economy class seats
            for (int row = hasBusinessClass ? 3 : 1; row <= 6; row++)
            {
                foreach (var col in columns)
                {
                    bool isWindow = (col == "A" || col == "E");
                    seats.Add(new Seat
                    {
                        FlightId = flightId,
                        SeatNo = $"{row}{col}",
                        Type = "Normal",
                        Price = isWindow ? economyFare * 1.2 : economyFare,
                        Color = isWindow ? "#B5F7FF" : "#87CEEB",
                        IsBooked = false
                    });
                    seatNumber++;
                    if (seatNumber > totalSeats) break;
                }
                if (seatNumber > totalSeats) break;
            }

            return seats;
        }

        public IActionResult PnrLookup() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PnrLookup(string pnr)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.PNR == pnr);
            if (booking == null)
            {
                ViewBag.Error = "No booking found for this PNR.";
                return View();
            }
            var flight = _context.Flights.Find(booking.FlightId);
            ViewBag.Booking = booking;
            ViewBag.Flight = flight;
            return View();
        }

        [HttpGet]
        public IActionResult MostVisitedFlights()
        {
            var flights = _context.Flights.ToList();
            return View(flights);
        }

        [HttpGet]
        public IActionResult FlightDetails(int id)
        {
            var flight = _context.Flights.FirstOrDefault(f => f.FlightId == id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        [HttpGet]
        public IActionResult DownloadCancellationPolicy()
        {
            var pdfBytes = CancellationPolicyPdfGenerator.Generate();
            return File(pdfBytes, "application/pdf", "SkyHigh_CancellationPolicy.pdf");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectFlight(int DepartureFlightId, string TripType, DateTime? ReturnDate)
        {
            // Validate
            if (TripType != "RoundTrip" || !ReturnDate.HasValue)
            {
                // For safety: fallback to booking directly if user picks one-way
                return RedirectToAction("Book", "Booking", new { flightId = DepartureFlightId });
            }

            // Save selected outbound flight info temporarily
            TempData["DepartureFlightId"] = DepartureFlightId;
            TempData["TripType"] = TripType;
            TempData["ReturnDate"] = ReturnDate.Value.ToString("yyyy-MM-dd");

            // Get outbound flight details
            var outboundFlight = _context.Flights.FirstOrDefault(f => f.FlightId == DepartureFlightId);
            if (outboundFlight == null)
            {
                return RedirectToAction("Search");
            }

            // Find return flights that go opposite direction
            var returnFlights = _context.Flights
                .Where(f => f.Source == outboundFlight.Destination
                            && f.Destination == outboundFlight.Source
                            && f.DepartureTime.Date == ReturnDate.Value.Date)
                .ToList();

            // Forward to SelectReturnFlight page
            ViewBag.OutboundFlight = outboundFlight;
            ViewBag.ReturnDate = ReturnDate;
            return View("SelectReturnFlight", returnFlights);
        }
    }
}





