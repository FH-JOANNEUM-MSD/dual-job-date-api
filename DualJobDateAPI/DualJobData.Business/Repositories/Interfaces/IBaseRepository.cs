namespace DualJobData.BusinessLogic.Repositories.Interfaces
{
    public interface IBaseRepository<T> : IDisposable
    {
        T GetById(int id);
        IQueryable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        void Save();
    }
}
