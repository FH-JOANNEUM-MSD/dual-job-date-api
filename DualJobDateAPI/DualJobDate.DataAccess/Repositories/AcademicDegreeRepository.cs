using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.DataAccess.Repositories;

public class AcademicDegreeRepository(AppDbContext dbContext) : BaseRepository<AcademicDegree>(dbContext);