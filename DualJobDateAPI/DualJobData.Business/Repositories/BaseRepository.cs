using DualJobData.BusinessLogic.Entities.Base;
using DualJobData.BusinessLogic.Repositories.Interfaces;
using DualJobData.DataAccess;

namespace DualJobData.BusinessLogic.Repositories
{
    public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T>
        where T : IBaseEntity
    {
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
            ((IDisposable)context).Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
