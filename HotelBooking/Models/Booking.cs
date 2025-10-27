using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RoomId { get; set; }
        [Required]
        public DateTime checkInTime { get; set; }
        [Required]
        public DateTime checkOutTime { get; set; }
        [Required]
        public double TotalPrice { get; set; }
        public bool Gym {  get; set; }
        public bool SPA {  get; set; }
        [ForeignKey("client")]
        public int ClientId { get; set; }
        public Client client { get; set; }
        public virtual List<Room> rooms { get; set; }

        //for many to many
        public virtual List<Admin> Admins { get; set; } = new();

    }
}
