using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.DataAccess.Repositories;

public class StudentCompanyRepository(AppDbContext dbContext)
    : BaseRepository<StudentCompany>(dbContext), IStudentCompanyRepository
{
    
}