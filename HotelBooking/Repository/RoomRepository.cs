using HotelBooking.IRepository;
using HotelBooking.Models;

namespace HotelBooking.Repository
{
    public class RoomRepository : GenaricRepository<Room>, IRoomRepository
    {
        public RoomRepository(HotelDbContext hotelDbContext) : base(hotelDbContext) { }
        public List<Room> GetAvailableRooms()
        {
            var availableRooms = GetAll().Where(r => r.IsAvailability == true).ToList();
            return availableRooms;
        }
    }
}
