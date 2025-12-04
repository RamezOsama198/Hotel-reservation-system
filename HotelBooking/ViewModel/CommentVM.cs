using System.ComponentModel.DataAnnotations;

namespace HotelBooking.ViewModel
{
    public class CommentVM
    {
        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string Title { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "Please sir you do not have to explain all the Details")]
        [MinLength(10, ErrorMessage = "please explain more")]
        public string Description { get; set; }
        public string ClientName { get; set; }
    }
}
