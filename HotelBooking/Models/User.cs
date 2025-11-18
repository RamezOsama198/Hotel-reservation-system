using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace HotelBooking.Models
{
    public class User: IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
