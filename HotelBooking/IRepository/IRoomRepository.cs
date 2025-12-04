using HotelBooking.Models;
using HotelBooking.Repository;

namespace HotelBooking.IRepository
{
    public interface IRoomRepository : IGenaricRepository<Room>
    {
        public List<Room> GetAvailableRooms();
        
        public List<Room> GetRoomsByStuffId(int stuffId);
    }
}
