using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class Admin : User
    {
        [Required]
        public string Role { get; set; }
        [Required]
        [Range(1000,50000)]
        public double Salary { get; set; }
        public virtual List<Stuff> Stuffs { get; set; } = new();
        // for many to many
        public virtual List<Booking> Bookings { get; set; } = new();
        public virtual List<Comment> Comments { get; set; } = new();
    }
}
