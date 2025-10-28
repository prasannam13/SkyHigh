using System;
using System.ComponentModel.DataAnnotations;

namespace SkyHigh.Models
{
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress,StringLength(150)]
        public string Email { get; set; }

        [Required, StringLength(20)]
        public string Phone { get; set; }

        [Required, StringLength(150)]
        public string Subject { get; set; }

        [Required, StringLength(1000)]
        public string Message { get; set; }

        public DateTime SubmittedOn { get; set; } = DateTime.Now;

        // Optional: Admin can reply
        public string? AdminReply { get; set; }
        public DateTime? RepliedOn { get; set; }
    }
}
