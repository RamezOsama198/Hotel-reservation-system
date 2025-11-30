using HotelBooking.Models;

namespace HotelBooking.IRepository
{
    public interface IClientRepository : IGenaricRepository<Client>
    {
        Client GetByNationalId(string nationalId);
    }
}
