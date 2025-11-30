using HotelBooking.Interfaces;
using HotelBooking.IRepository;
using HotelBooking.Models;
using HotelBooking.Repository;
using System.Xml.Linq;

namespace HotelBooking.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelDbContext _context;

        public IGenaricRepository<Booking> Bookings { get; private set; }
        public IRoomRepository Rooms { get; private set; }
        public IGenaricRepository<Stuff> Stuffs { get; private set; }
        public IGenaricRepository<Comment> Comments { get; private set; }
        public IClientRepository Clients { get; private set; }
        public IGenaricRepository<Admin> Admins { get; private set; }
        public UnitOfWork(HotelDbContext context)
        {
            _context = context;

            Bookings = new GenaricRepository<Booking>(context);
            Rooms = new RoomRepository(context);
            Stuffs = new GenaricRepository<Stuff>(context);
            Comments = new GenaricRepository<Comment>(context);
            Clients = new ClientRepository(context);
            Admins = new GenaricRepository<Admin>(context);
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
