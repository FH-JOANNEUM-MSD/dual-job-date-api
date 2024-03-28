using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.DataAccess.Repositories;

public class ActivityRepository(AppDbContext dbContext) : BaseRepository<Activity>(dbContext), IActivityRepository;