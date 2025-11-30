using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Admin
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        [Range(1000, 50000)]
        public double? Salary { get; set; }

        public virtual List<Stuff> Stuffs { get; set; } = new();
        public virtual List<Booking> Bookings { get; set; } = new();
        public virtual List<Comment> Comments { get; set; } = new();
    }
}