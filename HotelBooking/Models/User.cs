using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
    }
}