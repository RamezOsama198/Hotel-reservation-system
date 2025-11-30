using HotelBooking.Models;
using HotelBooking.Repository;

namespace HotelBooking.IRepository
{
    public interface IRoomRepository : IGenaricRepository<Room>
    {
        List<Room> GetAvailableRooms();
    }
}
