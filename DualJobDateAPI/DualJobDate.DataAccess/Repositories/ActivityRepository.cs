using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.DataAccess.Repositories;

public class ActivityRepository(AppDbContext dbContext) : BaseRepository<Activity>(dbContext);