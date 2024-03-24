using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.BusinessObjects.Entities.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IInstitutionRepository InstitutionRepository { get; }
        Task SaveChanges();
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
