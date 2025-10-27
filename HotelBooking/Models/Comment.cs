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
        [StringLength(1000,ErrorMessage ="Please sir you do not have to explain all the tetails")]
        [MinLength(10,ErrorMessage ="please explain more")]
        public string Description { get; set; }
        [ForeignKey("client")]
        public int ClientId { get; set; }
        public Client client { get; set; }
        //for many to many
        public virtual List<Admin> Admins { get; set; } = new();
    }
}
