using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime checkInTime { get; set; }
        [Required]
        public DateTime checkOutTime { get; set; }
        [Required]
        public double TotalPrice { get; set; }
        [Required]
        public int NumberOfGuests { get; set; }
        public bool Gym {  get; set; }
        public bool SPA {  get; set; }
        [ForeignKey("client")]
        public string? ClientId { get; set; }
        public virtual Client? client { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCheckedOut { get; set; }
        public bool IsExpired { get; set; } = false;
        public virtual List<Room>? rooms { get; set; }
        //for many to many
        public virtual List<Admin> Admins { get; set; } = new();

    }
}
