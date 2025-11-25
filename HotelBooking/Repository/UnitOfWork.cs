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
        public IGenaricRepository<Room> Rooms { get; private set; }
        public IGenaricRepository<Stuff> Stuffs { get; private set; }
        public IGenaricRepository<Comment> Comments { get; private set; }

        public UnitOfWork(HotelDbContext context)
        {
            _context = context;

            Bookings = new GenaricRepository<Booking>(context);
            Rooms = new GenaricRepository<Room>(context);
            Stuffs = new GenaricRepository<Stuff>(context);
            Comments = new GenaricRepository<Comment>(context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
