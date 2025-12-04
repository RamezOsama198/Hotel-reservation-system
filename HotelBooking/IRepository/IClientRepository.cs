using HotelBooking.Models;

namespace HotelBooking.IRepository
{
    public interface IClientRepository : IGenaricRepository<Client>
    {
        public Client GetByNationalId(string nationalId);
    }
}
