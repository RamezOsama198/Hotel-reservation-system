using HotelBooking.IRepository;
using HotelBooking.Models;

namespace HotelBooking.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenaricRepository<Booking> Bookings { get; }
        IRoomRepository Rooms { get; }
        IStuffRepository Stuffs { get; }
        IGenaricRepository<Comment> Comments { get; }
        IClientRepository Clients { get; }
        IGenaricRepository<Admin> Admins { get; }
    }
}
