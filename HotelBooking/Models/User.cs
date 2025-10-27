using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public abstract class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(2)]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"[a-z]{2,10}@gmail\.com")]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }// string because some phone number start with + outside & not a value
        [Required]
        [StringLength(4)]
        public string Password { get; set; }

    }
}
