using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Stuff
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(3)]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        [Range(1000,50000)]
        public double Salary { get; set; }
        [Required]
        public string JopTitle { get; set; }
        [Required]
        public string Gender { get; set; }
        [ForeignKey("admin")]
        public int AdminId { get; set; }
        public Admin admin { get; set; }
        //for many to many
        public List<Room> Rooms { get; set; }

    }
}
