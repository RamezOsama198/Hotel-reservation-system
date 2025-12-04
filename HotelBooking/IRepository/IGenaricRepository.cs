using HotelBooking.Models;

namespace HotelBooking.IRepository
{
    public interface IGenaricRepository<TableModel> where TableModel : class
    {
        public List<TableModel> GetAll();
        public TableModel GetById(int id);
        public void Insert(TableModel model);
        public void Update(TableModel model);
        public void Delete(int id);
        public void Delete(string id);
        public TableModel GetById(string id);
        public void Attach(TableModel model);
    }
}
