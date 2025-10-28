
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing; // <- required for ImageSource
using SkyHigh.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace SkyHigh.Helpers
{
    public static class TicketPdfGenerator
    {
        public static byte[] Generate(Booking booking, Flight flight, List<Passenger> passengers)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Page setup
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // HEADER
                    page.Header().PaddingBottom(20).Element(header =>
                    {
                        header.Row(row =>
                        {
                            // --- Left: Logo or fallback text ---
                            row.RelativeItem(0.25f).AlignMiddle().AlignCenter().Element(c =>
                            {
                                try
                                {
                                    using var logoStream = File.OpenRead("wwwroot/images/logo1.png");

                                    //  Proper way: wrap inside container for scaling
                                    c.Container().Height(50).AlignLeft().Image(logoStream).FitWidth();
                                }
                                catch
                                {
                                    // ✅Fallback: text header when logo missing
                                    c.Column(col =>
                                    {
                                        col.Item().Text("SkyHigh Airlines")
                                            .Bold()
                                            .FontSize(24)
                                            .FontColor(Colors.Blue.Medium);

                                        col.Item().Text("Your trusted partner for smooth air travel")
                                            .FontSize(11)
                                            .FontColor(Colors.Grey.Medium);
                                    });
                                }

                            });

                            // --- Right: PNR and Date ---
                            row.RelativeItem(0.75f).AlignRight().Column(col =>
                            {
                                col.Item().Text($"PNR: {booking.PNR}")
                                    .Bold()
                                    .FontColor(Colors.Blue.Medium);

                                col.Item().Text($"{booking.BookingDate:dd MMM yyyy}")
                                    .FontSize(10)
                                    .FontColor(Colors.Grey.Medium);
                            });
                        });
                    });



                    // CONTENT
                    page.Content().PaddingVertical(18).Column(col =>
                    {
                        col.Spacing(12);

                        // Section title (container for border/padding)
                        col.Item().Element(c =>
                        {
                            c.PaddingBottom(6)
                             .BorderBottom(1)
                             .BorderColor(Colors.Grey.Lighten2)
                             .Element(inner => inner.Text("Flight & Booking Details")
                                                     .FontSize(14)
                                                     .Bold()
                                                     .FontColor(Colors.Blue.Darken2));
                        });

                        // Flight + Booking info row
                        col.Item().Row(row =>
                        {
                            // Left column: flight info
                            row.RelativeItem().Column(left =>
                            {
                                left.Spacing(4);
                                left.Item().Column(flightInfo =>
                                {
                                    if (flight != null)
                                    {
                                        flightInfo.Spacing(2);
                                        flightInfo.Item().Text($"Flight No: {flight.FlightNo}").Bold();
                                        flightInfo.Item().Text($"Route: {flight.Source} → {flight.Destination}");
                                        flightInfo.Item().Text($"Departure: {flight.DepartureTime:dd-MM-yyyy HH:mm}");
                                        flightInfo.Item().Text($"Arrival: {flight.ArrivalTime:dd-MM-yyyy HH:mm}");
                                    }
                                    else
                                    {
                                        flightInfo.Item().Text("Flight details not available")
                                            .FontColor(Colors.Red.Medium);
                                    }
                                });

                            });

                            // Right column: booking info
                            row.RelativeItem().Column(right =>
                            {
                                right.Spacing(4);
                                right.Item().Column(bookingInfo =>
                                {
                                    bookingInfo.Spacing(2);
                                    bookingInfo.Item().Text($"Booking ID: {booking.BookingId}");
                                    bookingInfo.Item().Text($"Seats: {booking.SeatsBooked}");
                                    bookingInfo.Item().Text($"Status: {booking.Status}");
                                    bookingInfo.Item().Text($"Booking Date: {booking.BookingDate:dd-MM-yyyy HH:mm}");
                                    bookingInfo.Item().Text($"Total Amount: ₹{booking.TotalAmount:F2}");
                                });

                            });
                        });

                        // Passenger table
                        if (passengers != null && passengers.Count > 0)
                        {
                            col.Item().Element(c =>
                            {
                                c.PaddingTop(10)
                                 .PaddingBottom(6)
                                 .BorderBottom(1)
                                 .BorderColor(Colors.Grey.Lighten2)
                                 .Element(inner => inner.Text("Passenger Details")
                                                        .Bold().FontSize(14).FontColor(Colors.Blue.Darken2));
                            });

                            col.Item().Element(c =>
                            {
                                c.Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(30);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                    });

                                    // Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("#").SemiBold();
                                        header.Cell().Element(CellStyle).Text("Name").SemiBold();
                                        header.Cell().Element(CellStyle).Text("Age").SemiBold();
                                        header.Cell().Element(CellStyle).Text("Gender").SemiBold();
                                        header.Cell().Element(CellStyle).Text("Seat No").SemiBold();
                                    });

                                    int idx = 1;
                                    foreach (var p in passengers)
                                    {
                                        table.Cell().Element(CellStyle).Text(idx++.ToString());
                                        table.Cell().Element(CellStyle).Text(p.Name);
                                        table.Cell().Element(CellStyle).Text(p.Age.ToString());
                                        table.Cell().Element(CellStyle).Text(p.Gender);
                                        table.Cell().Element(CellStyle).Text(p.SeatNo);
                                    }
                                });
                            });
                        }

                        // Summary box
                        col.Item().Element(c =>
                        {
                            c.PaddingTop(12)
                             .Padding(10)
                             .Border(1)
                             .BorderColor(Colors.Grey.Lighten2)
                             .Background(Colors.Grey.Lighten4)
                             .Column(sum =>
                             {
                                 sum.Spacing(4);
                                 sum.Item().Element(x => x.Text("Summary").Bold().FontColor(Colors.Blue.Medium).FontSize(13));
                                 sum.Item().Element(x => x.Text($"PNR: {booking.PNR}"));
                                 sum.Item().Element(x => x.Text($"Flight: {flight?.FlightNo ?? "-"}"));
                                 sum.Item().Element(x => x.Text($"Total Paid: ₹{booking.TotalAmount:F2}"));
                                 sum.Item().Element(x => x.Text($"Booking Status: {booking.Status}"));
                             });
                        });

                        // Travel guidelines (use Column to emulate bullet list)
                        col.Item().Element(c =>
                        {
                            c.PaddingTop(14).Column(guide =>
                            {
                                guide.Spacing(6);
                                guide.Item().Element(x => x.Text("✈️ Travel Guidelines").Bold().FontColor(Colors.Blue.Darken2).FontSize(13));
                                guide.Item().Element(x =>
                                {
                                    x.Column(bullets =>
                                    {
                                        bullets.Spacing(4);
                                        bullets.Item().Text("• Arrive at least 2 hours before departure.");
                                        bullets.Item().Text("• Carry a valid government photo ID and your e-ticket.");
                                        bullets.Item().Text("• Ensure baggage is within airline limits.");
                                        bullets.Item().Text("• Boarding gates close 20 minutes before departure.");
                                        bullets.Item().Text("• Use SkyHigh mobile app for web check-in and updates.");
                                    });
                                });
                            });
                        });
                    });

                    // FOOTER
                    page.Footer().Element(footer =>
                    {
                        footer.PaddingTop(10).Column(col =>
                        {
                            col.Spacing(6);

                            // thin blue line (as divider)
                            col.Item().Element(c => c.PaddingVertical(3).BorderBottom(1).BorderColor(Colors.Blue.Medium).ExtendHorizontal());

                            col.Item().AlignCenter().Element(c =>
                            {
                                c.Text("Thank you for flying with SkyHigh Airlines ✈️")
                                    .FontColor(Colors.Blue.Medium)
                                    .FontSize(10)
                                    .Bold();
                            });

                            col.Item().AlignCenter().Element(c =>
                            {
                                c.Text("For support: skyhighairlinee@gmail.com | www.skyhighairlines.com")
                                    .FontSize(9).FontColor(Colors.Grey.Medium);
                            });

                            col.Item().AlignCenter().Element(c =>
                            {
                                c.Text("Safe Travels • Smooth Journeys • Memorable Flights")
                                    .FontSize(9).FontColor(Colors.Grey.Darken1);
                            });
                        });
                    });
                });
            });

            using (var ms = new MemoryStream())
            {
                doc.GeneratePdf(ms);
                return ms.ToArray();
            }
        }

        // Simple cell style for table rows
        static IContainer CellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor("#DDD")
                .PaddingVertical(4)
                .PaddingHorizontal(6)
                .AlignMiddle();
        }
    }
}


