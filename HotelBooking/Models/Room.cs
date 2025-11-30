using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RoomType { get; set; }
        [Required]
        [Range(2,10)] // <<-------
        public int MaxPeople { get; set; }
        [Required]
        public bool IsAvailability { get; set; }
        [Required]
        public double PricePerDay { get; set; }

        [ForeignKey("Booking")]
        public int? BookingId { get; set; }
        public Booking Booking { get; set; }
        //for many to many
        public List<Stuff> Stuffs { get; set; }
    }
}
