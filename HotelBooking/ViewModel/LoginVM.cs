using System.ComponentModel.DataAnnotations;

namespace HotelBooking.ViewModel
{
    public class LoginVM
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]([a-zA-Z0-9\.]{0,28}[a-zA-Z0-9])?@gmail\.com$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
