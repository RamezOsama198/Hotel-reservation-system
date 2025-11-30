using System.ComponentModel.DataAnnotations;

namespace HotelBooking.ViewModel
{
    public class AdminVM
    {
        [Required]
        [StringLength(50)]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]([a-zA-Z0-9\.]{0,28}[a-zA-Z0-9])?@gmail\.com$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Range(1000, 50000)]
        public double? Salary { get; set; }

        [Required]
        public string Role { get; set; }
    }
}