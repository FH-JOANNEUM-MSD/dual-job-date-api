using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.DataAccess.Repositories;

public class CompanyActivityRepository(AppDbContext dbContext) : BaseRepository<CompanyActivity>(dbContext), ICompanyActivityRepository;