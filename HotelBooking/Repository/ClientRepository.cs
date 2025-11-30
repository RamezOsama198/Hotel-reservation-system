using HotelBooking.IRepository;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repository
{
    public class ClientRepository : GenaricRepository<Client>, IClientRepository
    {
        public ClientRepository(HotelDbContext hotelDbContext) : base(hotelDbContext) { }
        public Client GetByNationalId(string nationalId)
        {
            var client = GetAll().FirstOrDefault(c => c.NationalId == nationalId);
            return client;
        }
    }
}
