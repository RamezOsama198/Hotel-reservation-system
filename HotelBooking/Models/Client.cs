using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Address { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string NationalId { get; set; }

        public virtual List<Comment> comments { get; set; } = new();
        public virtual List<Booking> bookings { get; set; } = new();
    }
}
