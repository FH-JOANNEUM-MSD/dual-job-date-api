using DualJobData.BusinessLogic.Entities;
using DualJobData.DataAccess;
using DualJobDateAPI.Repository.Interfaces;

namespace DualJobDateAPI.Repository
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : BaseEntity
    {
        private readonly AppDbContext _context;
        public readonly int? _stationId;
        public readonly int _tenantId;

        public BaseRepository(AppDbContext context, StationTenantConfig stationTenantConfig)
        {
            _context = context;
            _stationId = stationTenantConfig.StationId;
            _tenantId = stationTenantConfig.TenantId;
        }

        public void Add(T entity)
        {
            throw new NotImplementedException("Add to db not implemented!");
        }

        public void Delete(int id)
        {
            throw new NotImplementedException("Delete from db not implemented!");
        }

        public IQueryable<T> GetAll()
        {
            throw new NotImplementedException("GetAll from db not implemented!");
        }

        public T GetById(int id)
        {
            throw new NotImplementedException("Get by id from db not implemented!");
        }

        public void Save()
        {
            throw new NotImplementedException("Save entity from db not implemented!");
        }

        public void Update(T entity)
        {
            throw new NotImplementedException("Update entity from db not implemented!");
        }
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
