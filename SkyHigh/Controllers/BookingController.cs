using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using SkyHigh.Data;

using SkyHigh.Helpers;

using SkyHigh.Models;

using System;

using System.Collections.Generic;

using System.Linq;



namespace SkyHigh.Controllers

{

    public class BookingController : Controller

    {

        private readonly ApplicationDbContext _context;

        private string? selectedClass;



        public BookingController(ApplicationDbContext context) => _context = context;



        // -------------------------------------------------------

        // GET: Booking/Book

       

        public IActionResult Book(int flightId, int? returnFlightId, string tripType)

        {

            var flight = _context.Flights.Include(f => f.Seats).FirstOrDefault(f => f.FlightId == flightId);

            if (flight == null) return NotFound();

            // Fetch seats and mark which are already booked

            var seats = _context.Seats

                .Where(s => s.FlightId == flightId)

                .Select(s => new Seat

                {

                    SeatId = s.SeatId,

                    FlightId = s.FlightId,

                    SeatNo = s.SeatNo,

                    Type = s.Type,

                    Price = s.Price,

                    Color = s.Color,

                    IsBooked = s.IsBooked

                })

                .ToList();

            ViewBag.Flight = flight;

            ViewBag.Seats = seats;

            ViewBag.TripType = tripType ?? "OneWay";



            if (tripType == "RoundTrip" && returnFlightId.HasValue)

            {

                var returnFlight = _context.Flights.FirstOrDefault(f => f.FlightId == returnFlightId.Value);

                var returnSeats = _context.Seats.Where(s => s.FlightId == returnFlightId.Value).

                    Select(s => new Seat

                    {

                        SeatId = s.SeatId,

                        FlightId = s.FlightId,

                        SeatNo = s.SeatNo,

                        Type = s.Type,

                        Price = s.Price,

                        Color = s.Color,

                        IsBooked = s.IsBooked

                    })

                    .ToList();



                ViewBag.ReturnFlight = returnFlight;

                ViewBag.ReturnSeats = returnSeats;

            }



            return View();

        }



        // -------------------------------------------------------

        // POST: Booking/Book

       

        [HttpPost]

        [ValidateAntiForgeryToken]

        public IActionResult Book(int flightId, int? returnFlightId, string TripType,

            List<PassengerInputModel> departurePassengers, List<PassengerInputModel> returnPassengers)

        {

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) return RedirectToAction("Login", "Account");



            // ✅ Create departure booking

            var depBooking = CreateBookingRecord(userId.Value, flightId, departurePassengers, TripType, "Departure");



            // ✅ Create return booking if round trip

            if (TripType == "RoundTrip" && returnFlightId.HasValue && returnPassengers != null && returnPassengers.Count > 0)

            {

                var retBooking = CreateBookingRecord(userId.Value, returnFlightId.Value, returnPassengers, TripType, "Return");

            }



            // ✅ Redirect user to confirmation first (not payment directly)

            return RedirectToAction("Confirmation", new { bookingId = depBooking.BookingId });

        }

        [HttpGet]

        public IActionResult Confirmation(int bookingId)

        {

            var booking = _context.Bookings.Include(b => b.Flight).FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();



            ViewBag.Flight = _context.Flights.FirstOrDefault(f => f.FlightId == booking.FlightId);

            if (booking.ReturnFlightId.HasValue)

                ViewBag.ReturnFlight = _context.Flights.FirstOrDefault(f => f.FlightId == booking.ReturnFlightId.Value);



            return View(booking);

        }



        [HttpPost]

        [ValidateAntiForgeryToken]

        public IActionResult ConfirmationPost(int bookingId)

        {

            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();



            booking.Status = "Pending Aadhaar Verification";

            _context.SaveChanges();



            return RedirectToAction("Aadhar", new { bookingId });

        }

        // -------------------------------------------------------

        // Create booking record (core logic)


        private Booking CreateBookingRecord(int userId, int flightId, List<PassengerInputModel> passengers, string tripType, string bookingType)

        {

            var booking = new Booking

            {

                UserId = userId,

                FlightId = flightId,

                PNR = GeneratePNR(),

                BookingDate = DateTime.Now,

                Status = "Pending Payment",

                Class = selectedClass ?? "Economy",

                TripType = tripType ?? "OneWay",

                SeatsBooked = string.Empty,

                TotalAmount = 0,

                //Ensure Db not null column gets a non-null value

                AadharNumber = string.Empty,

                AadharVerified = false

            };



            _context.Bookings.Add(booking);

            _context.SaveChanges();



            double totalAmount = 0;

            var allSeats = new List<string>();



            if (passengers != null && passengers.Any())

            {

                foreach (var p in passengers)

                {

                    if (p.SeatNos == null || p.SeatNos.Length == 0) continue;



                    foreach (var seatNo in p.SeatNos)

                    {

                        var seat = _context.Seats.FirstOrDefault(s => s.FlightId == flightId && s.SeatNo == seatNo);

                        if (seat == null || seat.IsBooked)

                        {

                            continue;

                        }



                        totalAmount += seat.Price;

                        allSeats.Add(seat.SeatNo);

                        seat.IsBooked = true;

                        seat.Color = "#FF5C5C";//Mark booked seats visually red for admin

                        _context.Seats.Update(seat);



                        var passenger = new Passenger

                        {

                            Name = p.Name,

                            Age = p.Age,

                            Gender = p.Gender,

                            SeatNo = seat.SeatNo,

                            Class = seat.Type ?? "Economy",

                            BookingId = booking.BookingId

                        };



                        _context.Passengers.Add(passenger);

                    }

                }



                _context.SaveChanges();

            }

            booking.TotalAmount = totalAmount;

            booking.SeatsBooked = string.Join(",", allSeats.Distinct());

            _context.SaveChanges();

            return booking;
        }

        // -------------------------------------------------------

        // NEW: AADHAR VERIFICATION PAGES

        [HttpGet]

        public IActionResult Aadhar(int bookingId)

        {

            var booking = _context.Bookings.Include(b => b.Passengers).FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();



            var model = new AadharViewModel { BookingId = bookingId };

            ViewBag.Booking = booking;

            return View(model);

        }



        [HttpPost]

        [ValidateAntiForgeryToken]

        public IActionResult Aadhar(AadharViewModel model)

        {

            // Validate form inputs

            if (!ModelState.IsValid)

            {

                var bookingInvalid = _context.Bookings

                    .Include(b => b.Passengers)

                    .FirstOrDefault(b => b.BookingId == model.BookingId);



                ViewBag.Booking = bookingInvalid;

                return View(model);

            }



            // Find booking by ID

            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == model.BookingId);

            if (booking == null)

                return NotFound("Booking not found.");



            // ✅ Save Aadhaar info on booking

            booking.AadharNumber = model.AadharNumber;

            booking.AadharVerified = true;



            // ✅ Update status so MyBookings shows "Pending Payment"

            booking.Status = "Pending Payment";



            _context.SaveChanges();



            // ✅ Redirect to Payment page

            return RedirectToAction("Payment", new { bookingId = model.BookingId });

        }

        // -------------------------------------------------------

        // SHOW PAYMENT PAGE


        [HttpGet]

        public IActionResult Payment(int bookingId)

        {

            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)

                return NotFound();



            //  Require Aadhaar verification

            if (!booking.AadharVerified)

            {

                return RedirectToAction("Aadhar", new { bookingId });

            }



            return View(booking);

        }



        // --------------------

        // HANDLE PAYMENT FORM

        [HttpPost]

        public IActionResult Payment(int bookingId, string cardNo, string cardName, string expiry, string cvv)

        {

            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)

                return NotFound();



            double gstRate = booking.Class?.Trim().ToLower() == "business" ? 0.18 : 0.05;

            double gstAmount = booking.TotalAmount * gstRate;

            double totalPayable = booking.TotalAmount + gstAmount;



            var payment = new Payment

            {

                BookingId = booking.BookingId,

                Amount = booking.TotalAmount,

                GST = gstAmount,

                GstRate = gstRate * 100,

                TotalAmount = totalPayable,

                PaymentDate = DateTime.Now,

                Method = "Card",

                Status = "Success"

            };



            _context.Payments.Add(payment);

            booking.Status = "Confirmed";

            _context.SaveChanges();



            return RedirectToAction("ViewBooking", new { bookingId = booking.BookingId });

        }



        // -------------------------------------------------------

        // GET: ViewBooking

        public IActionResult ViewBooking(int? bookingId)

        {

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) return RedirectToAction("Login", "Account");



            if (bookingId != null)

            {

                var booking = _context.Bookings

                    .Include(b => b.Flight)

                    .Where(b => b.BookingId == bookingId && b.UserId == userId)

                    .Select(b => new Booking

                    {

                        BookingId = b.BookingId,

                        UserId = b.UserId,

                        FlightId = b.FlightId,

                        PNR = b.PNR,

                        Class = b.Class,

                        SeatsBooked = b.SeatsBooked,

                        BookingDate = b.BookingDate,

                        TotalAmount = b.TotalAmount,

                        Status = b.Status,

                        Flight = b.Flight,

                        Passengers = _context.Passengers.Where(p => p.BookingId == b.BookingId).ToList()

                    })

                    .FirstOrDefault();



                if (booking == null) return NotFound();



                var flight = _context.Flights.Find(booking.FlightId);

                var payment = _context.Payments.FirstOrDefault(p => p.BookingId == bookingId);



                ViewBag.Booking = booking;

                ViewBag.Flight = flight;

                ViewBag.Payment = payment;

                return View("ViewBooking");

            }



            var bookings = _context.Bookings.Where(b => b.UserId == userId).ToList();

            return View("MyBookings", bookings);

        }



        public IActionResult MyBookings()

        {

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) return RedirectToAction("Login", "Account");



            var bookings = _context.Bookings

                .Where(b => b.UserId == userId)

                .OrderByDescending(b => b.BookingDate)

                .Include(b => b.Flight)

                .ToList();



            return View(bookings);

        }



        private string GeneratePNR()

        {

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var rand = new Random();

            return new string(Enumerable.Repeat(chars, 8).Select(s => s[rand.Next(s.Length)]).ToArray());

        }



        public class PassengerInputModel

        {

            public string Name { get; set; } = string.Empty;

            public int Age { get; set; }

            public string Gender { get; set; } = string.Empty;

            public string[] SeatNos { get; set; } = Array.Empty<string>();

        }



        [HttpGet]

        public IActionResult DownloadTicket(int bookingId)

        {

            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null) return NotFound("Booking not found.");



            var flight = _context.Flights.FirstOrDefault(f => f.FlightId == booking.FlightId);

            var passengers = _context.Passengers.Where(p => p.BookingId == booking.BookingId).ToList();

            var pdfBytes = TicketPdfGenerator.Generate(booking, flight, passengers);



            string status = booking.Status?.ToLower() == "cancelled" ? "CANCELLED_" : "";

            return File(pdfBytes, "application/pdf", $"{status}Ticket_{booking.PNR}.pdf");

        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult AjaxCancelAndDownload([FromBody] CancelRequest req)
        {
            var booking = _context.Bookings.Find(req.bookingId);
            if (booking == null)
                return Json(new { success = false, message = "Booking not found." });

            var flight = _context.Flights.Find(booking.FlightId);
            if (DateTime.Now.AddHours(3) > flight.DepartureTime)
            {
                return Json(new { success = false, message = "Cannot cancel within 3 hours of departure." });
            }

            booking.Status = "Cancelled";
            _context.SaveChanges();

            foreach (var seatNo in booking.SeatsBooked.Split(","))
            {
                var seat = _context.Seats.FirstOrDefault(s => s.FlightId == flight.FlightId && s.SeatNo == seatNo);
                if (seat != null)
                {
                    seat.IsBooked = false;
                    seat.Color = seat.Type == "VIP" ? "#FFD700" : "#87CEEB";
                }
            }
            _context.SaveChanges();

            var passengers = _context.Passengers.Where(p => p.BookingId == booking.BookingId).ToList();
            var pdfBytes = TicketPdfGenerator.Generate(booking, flight, passengers);
            string filename = $"CANCELLED_Ticket_{booking.PNR}.pdf";
            string base64Pdf = Convert.ToBase64String(pdfBytes);

            return Json(new { success = true, filename = filename, pdfBase64 = base64Pdf });
        }

        public class CancelRequest { public int bookingId { get; set; } }

    }

}



