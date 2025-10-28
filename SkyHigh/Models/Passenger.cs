using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkyHigh.Models
{
    public class Passenger
    {
        public int PassengerId { get; set; }
        public int BookingId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Range(0,150,ErrorMessage ="Age must be between 0 and 150.")]
        public int Age { get; set; }
        [Required]
        [StringLength(10)]
        public string SeatNo { get; set; }
        [Required]
        [StringLength(10)]
        public string Gender { get; set; }
        [NotMapped]
        public List<string> SeatNos { get; set; }
        //  property to support seat type (Economy/Business)
        public string Class { get; set; } = "Economy";


       
        public Booking Booking { get; set; }
    }
}

