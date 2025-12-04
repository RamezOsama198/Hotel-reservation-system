using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Client
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Address { get; set; }

        [Required]
        [StringLength(14, MinimumLength = 14)]
        [RegularExpression("^[0-9]{14}$", ErrorMessage = "National ID must be 14 digits.")]
        public string NationalId { get; set; }

        public virtual List<Comment> comments { get; set; } = new();
        public virtual List<Booking> bookings { get; set; } = new();
    }
}
