
using System.Runtime.Intrinsics.X86;

using Microsoft.AspNetCore.Mvc;



namespace SkyHigh.Controllers

{

    [ApiController]

    [Route("[controller]/[action]")]

    public class ChatController : Controller

    {

        [HttpPost]

        public IActionResult GetResponse([FromBody] ChatRequest request)

        {

            string userMessage = request.Message.ToLower();

            string reply;



            if (userMessage.Contains("hello") || userMessage.Contains("hi"))

                reply = "Hi there! 👋 How can I assist you with your flight today?";

            else if (userMessage.Contains("book"))

                reply = "To book a ticket, please go to the 'Book Flight' section in the main menu.";

            else if (userMessage.Contains("cancel"))

                reply = "You can cancel your booking under 'My Bookings' → 'Cancel Ticket'.";

            else if (userMessage.Contains("refund"))

                reply = "Refunds are processed within 5–7 business days after cancellation.";

            else if (userMessage.Contains("help") || userMessage.Contains("agent"))

                reply = "A live agent will be available soon. Please leave your message here.";



            else if (userMessage.Contains("status") || userMessage.Contains("flight status"))

                reply = "You can check your flight status under 'My Bookings' → 'Flight Status'.";



            else if (userMessage.Contains("check-in") || userMessage.Contains("boarding pass"))

                reply = "Online check-in opens 24 hours before departure. You can download your boarding pass from 'My Bookings'.";



            else if (userMessage.Contains("baggage") || userMessage.Contains("luggage"))

                reply = "Economy class allows 15kg check-in and 7kg cabin baggage. Business class allows 30kg check-in.";



            else if (userMessage.Contains("id") || userMessage.Contains("documents"))

                reply = "Please carry a valid government-issued photo ID for check-in and boarding.";



            else if (userMessage.Contains("child") || userMessage.Contains("infant"))

                reply = "Infants under 2 years travel free with an adult. Children over 2 require a separate ticket.";



            else if (userMessage.Contains("delay") || userMessage.Contains("cancelled"))

                reply = "If your flight is delayed or cancelled, you will be notified via SMS and email. You can also check updates in 'My Bookings'.";

            else if (userMessage.Contains("seat") || userMessage.Contains("choose seat"))

                reply = "You can select your seat during booking or check-in. Visit 'My Bookings' → 'Manage Seat'.";

            else if (userMessage.Contains("payment") || userMessage.Contains("failed"))

                reply = "If your payment failed, please try again or contact support@skyhighairlines.com.";

            else if (userMessage.Contains("thank") || userMessage.Contains("thanks") || userMessage.Contains("thank you") || userMessage.Contains("ty"))

            {

                reply = "You're most welcome! 😊 I'm happy to help — have a great day!";

            }

            else if (userMessage.Contains("bye"))

            {

                reply = "Goodbye! ✈️ Have a pleasant day and safe travels!";

            }



            else

                reply = "I’m not sure about that. Please contact support@skyhighairlines.com.";



            return Json(new { reply });

        }



        public class ChatRequest

        {

            public string Message { get; set; }

        }

    }

}