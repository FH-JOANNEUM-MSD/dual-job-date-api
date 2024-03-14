using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.DataAccess.Repositories;

public class CompanyDetailsActivity(AppDbContext dbContext) : BaseRepository<CompanyDetails>(dbContext);