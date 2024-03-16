using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.DataAccess.Repositories;

public class CompanyRepository(AppDbContext dbContext) : BaseRepository<Company>(dbContext);