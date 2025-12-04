using HotelBooking.Models;

namespace HotelBooking.ViewModel
{
    public class BookingDetailsVM
    {
        public int Id { get; set; }
        public DateTime checkInTime { get; set; }
        public DateTime checkOutTime { get; set; }
        public int NumberOfGuests { get; set; }
        public double TotalPrice { get; set; }
        public bool Gym { get; set; }
        public bool SPA { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCheckedOut { get; set; }
        public string ClientName { get; set; }
        public string ClientNationalId { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
