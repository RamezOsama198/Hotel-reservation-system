using HotelBooking.Models;

namespace HotelBooking.IRepository
{
    public interface IStuffRepository : IGenaricRepository<Stuff>
    {
        public List<Stuff> GetStuffsByRoomId(int roomId);
    }
}
