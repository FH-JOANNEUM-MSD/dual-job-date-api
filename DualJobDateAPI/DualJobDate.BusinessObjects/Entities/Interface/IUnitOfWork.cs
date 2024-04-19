using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.BusinessObjects.Entities.Interface;

public interface IUnitOfWork : IDisposable
{
    IInstitutionRepository InstitutionRepository { get; }
    IAcademicProgramRepository AcademicProgramRepository { get; }
    IActivityRepository ActivityRepository { get; }
    ICompanyRepository CompanyRepository { get; }
    ICompanyDetailsRepository CompanyDetailsRepository { get; }
    ICompanyActivityRepository CompanyActivityRepository { get; }
    IAdressRepository AdressRepository { get; }


    Task SaveChanges();
    void BeginTransaction();
    void Commit();
    void Rollback();
}