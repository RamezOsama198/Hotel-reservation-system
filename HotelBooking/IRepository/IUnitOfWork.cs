using HotelBooking.IRepository;
using HotelBooking.Models;

namespace HotelBooking.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenaricRepository<Booking> Bookings { get; }
        IGenaricRepository<Room> Rooms { get; }
        IGenaricRepository<Stuff> Stuffs { get; }
        IGenaricRepository<Comment> Comments { get; }
    }
}
