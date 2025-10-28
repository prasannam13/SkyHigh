using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace SkyHigh.Helpers
{
    public static class CancellationPolicyPdfGenerator
    {
        public static byte[] Generate()
        {
            var policyText = @"SkyHigh Airlines Cancellation Policy

1. Cancellations made more than 72 hours before departure: Full refund minus ₹500 processing fee.
2. Cancellations made 24-72 hours before departure: 50% refund.
3. Cancellations made within 24 hours of departure: No refund.
4. No-show or missed flight: No refund.
5. Contact support for special cases.

Thank you for choosing SkyHigh Airlines!";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(16));
                    page.Header().Text("Cancellation Policies").Bold().FontSize(24).FontColor(Colors.Blue.Medium);
                    page.Content().Text(policyText);
                    page.Footer().AlignCenter().Text("© 2025 SkyHigh Airlines");
                });
            });

            using (var ms = new MemoryStream())
            {
                document.GeneratePdf(ms);
                return ms.ToArray();
            }
        }
    }
}