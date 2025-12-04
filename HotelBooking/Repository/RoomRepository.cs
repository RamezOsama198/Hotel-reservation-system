using HotelBooking.IRepository;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repository
{
    public class RoomRepository : GenaricRepository<Room>, IRoomRepository
    {
        private readonly HotelDbContext _context;
        public RoomRepository(HotelDbContext hotelDbContext) : base(hotelDbContext) 
        {
            _context = hotelDbContext;
        }
        public List<Room> GetAvailableRooms()
        {
            var availableRooms = GetAll().Where(r => r.IsAvailability == true).ToList();
            return availableRooms;
        }

        public List<Room> GetRoomsByStuffId(int stuffId)
        {
            var rooms = _context.Rooms.Include(r => r.Stuffs).Where(r => r.Stuffs.Any(s => s.Id == stuffId)).ToList();
            return rooms;
        }
    }
}
