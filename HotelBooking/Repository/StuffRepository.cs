using HotelBooking.IRepository;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repository
{
    public class StuffRepository : GenaricRepository<Stuff>, IStuffRepository
    {
        private readonly HotelDbContext _context;
        public StuffRepository(HotelDbContext hotelDbContext) : base(hotelDbContext) 
        {
            _context = hotelDbContext;
        }
        public List<Stuff> GetStuffsByRoomId(int roomId)
        {
            var stuffs = _context.Stuffs.Include(s => s.Rooms).Where(s => s.Rooms.Any(r => r.Id == roomId)).ToList();
            return stuffs;
        }
    }
}
