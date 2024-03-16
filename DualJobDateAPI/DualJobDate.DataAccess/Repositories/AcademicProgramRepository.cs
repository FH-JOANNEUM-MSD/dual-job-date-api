using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.DataAccess.Repositories;

public class AcademicProgramRepository(AppDbContext dbContext) : BaseRepository<AcademicProgram>(dbContext);