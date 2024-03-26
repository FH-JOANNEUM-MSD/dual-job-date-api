using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.DataAccess.Repositories;

public class CompanyDetailsRepository(AppDbContext dbContext) : BaseRepository<CompanyDetails>(dbContext), ICompanyDetailsRepository;