using System.ComponentModel.DataAnnotations;

namespace HotelBooking.ViewModel
{
    public class UserVM
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]([a-zA-Z0-9\.]{0,28}[a-zA-Z0-9])?@gmail\.com$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }


        public string AdminRole { get; set; }

        [Range(1000, 50000)]
        public double? Salary { get; set; }

        [StringLength(100, MinimumLength = 5)]
        public string? Address { get; set; }

        [StringLength(14, MinimumLength = 14, ErrorMessage = "NationalId in Egypt Must be 14 Number")]
        public string? NationalId { get; set; }
    }
}