using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [StringLength(1000,ErrorMessage ="Please sir you do not have to explain all the Details")]
        [MinLength(10,ErrorMessage ="please explain more")]
        public string Description { get; set; }
        [ForeignKey("client")]
        public string ClientId { get; set; }
        public Client client { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
