using HotelBooking.IRepository;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repository
{
    public class ClientRepository : GenaricRepository<Client>, IClientRepository
    {
        private readonly HotelDbContext _context;
        public ClientRepository(HotelDbContext hotelDbContext) : base(hotelDbContext) 
        {
            _context = hotelDbContext;
        }
        public Client GetByNationalId(string nationalId)
        {
            var client = _context.Clients.Include(c => c.User).FirstOrDefault(c => c.NationalId == nationalId.Trim());
            return client;
        }
    }
}
