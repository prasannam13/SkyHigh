using System;

using System.ComponentModel.DataAnnotations;

using System.Security.Cryptography;

using System.Text;

namespace SkyHigh.Models

{

    public class User

    {

        public int UserId { get; set; }

     

        [Required(ErrorMessage = "Name is required")]

        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]

        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]

        [EmailAddress(ErrorMessage = "Enter a valid email")]

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]

        [DataType(DataType.Date)]

        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Phone number is required")]

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a 10-digit phone number")]

        public string Phone { get; set; }

        [Required(ErrorMessage = "Please select a gender")]

        public string Gender { get; set; }

        public string Role { get; set; } = "User";

        public string? ProfilePicPath { get; set; }

        public static string HashPassword(string password)

        {

            using var sha256 = SHA256.Create();

            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();

        }

    }

}

