using DualJobData.BusinessLogic.Repositories.Interfaces;

namespace DualJobData.BusinessLogic.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        Task SaveChanges();
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
