using HotelBooking.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.ViewModel
{
    public class StuffVM
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [MinLength(3)]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }
        [Required]
        [Range(1000, 50000)]
        public double Salary { get; set; }
        [Required]
        [StringLength(20)]
        public string JopTitle { get; set; }
        [Required]
        public string Gender { get; set; }

        public List<Room>? Rooms { get; set; }
        public List<int>? SelectedRoomsIds { get; set; }
    }
}
