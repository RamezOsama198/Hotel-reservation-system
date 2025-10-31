using HotelBooking.IRepository;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repository
{
    public class GenaricRepository<TableModel> : IGenaricRepository<TableModel> where TableModel : class
    {
        private readonly HotelDbContext _hotelDbContext;
        private readonly DbSet<TableModel> _model;
        public GenaricRepository(HotelDbContext hotelDbContext)
        {
            _hotelDbContext = hotelDbContext;
            _model = hotelDbContext.Set<TableModel>();
        }

        public List<TableModel> Get()
        {
            List<TableModel> models = _model.AsNoTracking().ToList();
            return models;
        }

        public TableModel GetById(int id)
        {
            TableModel model = _model.Find(id);
            return model;
        }

        public void Insert(TableModel model)
        {
            _model.Add(model);
            _hotelDbContext.SaveChanges();
        }

        public void Update(TableModel model)
        {
            _model.Update(model);
            _hotelDbContext.SaveChanges();
        }
        public void Delete(int id)
        {
            TableModel model = GetById(id);
            _model.Remove(model);
            _hotelDbContext.SaveChanges();
        }
    }
}
