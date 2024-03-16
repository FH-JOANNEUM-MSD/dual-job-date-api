using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.DataAccess.Repositories;

public class UserTypeRepository(AppDbContext dbContext) : BaseRepository<UserType>(dbContext);