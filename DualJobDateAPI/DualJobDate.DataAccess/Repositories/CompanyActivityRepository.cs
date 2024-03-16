using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.DataAccess.Repositories;

public class CompanyActivityRepository(AppDbContext dbContext) : BaseRepository<CompanyActivity>(dbContext);