//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace SkyHigh.Models
//{
//    public class Flight
//    {
//        public int FlightId { get; set; }
//        [Required]
//        [StringLength(20)]
//        public string FlightNo { get; set; }
//        [Required]
//        [StringLength(20)]
//        public string Source { get; set; }
//        [Required]
//        [StringLength(20)]
//        public string Destination { get; set; }
//        [Required]
//        [DataType(DataType.DateTime)]
//        public DateTime DepartureTime { get; set; }
//        [Required]
//        [DataType(DataType.DateTime)]
//        public DateTime ArrivalTime { get; set; }
//        [Required]
//        [Range(1,10000,ErrorMessage ="Duration must be positive (minutes).")]
//        public int Duration { get; set; }
//        [Required]
//        [Range(0,double.MaxValue, ErrorMessage = "EconomyFare must be non-negative")]
//        public double EconomyFare { get; set; }
//        [Required]
//        [Range(0, double.MaxValue, ErrorMessage = "BusinessFare must be non-negative")]
//        public double BusinessFare { get; set; }
//        public List<Seat> Seats { get; set; } = new List<Seat>();

//        public bool HasBusinessClass { get; set; }
//    }
//}
using System;

using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

namespace SkyHigh.Models

{

    public class Flight

    {

        public int FlightId { get; set; }

        [Required(ErrorMessage = "Flight number is required")]

        public string FlightNo { get; set; }

        [Required(ErrorMessage = "Source is required")]

        public string Source { get; set; }

        [Required(ErrorMessage = "Destination is required")]

        public string Destination { get; set; }

        [Required(ErrorMessage = "Departure time is required")]

        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required")]

        public DateTime ArrivalTime { get; set; }

        [Range(1, 1000, ErrorMessage = "Duration must be between 1 and 1000 minutes")]

        public int Duration { get; set; }

        [Range(100, double.MaxValue, ErrorMessage = "Economy fare must be greater than 100")]

        public double EconomyFare { get; set; }

        [Range(100, double.MaxValue, ErrorMessage = "Business fare must be greater than 100")]

        public double BusinessFare { get; set; }

        public List<Seat> Seats { get; set; } = new List<Seat>();

        public bool HasBusinessClass { get; set; }

    }

}


