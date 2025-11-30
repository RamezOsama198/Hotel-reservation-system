using HotelBooking.Models;

namespace HotelBooking.IServicesLayer
{
    public interface IBookingServices
    {
        public double CalculateTotalPrice(Booking booking, List<int> roomIds);
        void ExpireOldBookings();
        void CreateBooking(Models.Booking booking, List<int> roomIds);
        public Booking GetByIdWithRooms(int id);
    }
}
