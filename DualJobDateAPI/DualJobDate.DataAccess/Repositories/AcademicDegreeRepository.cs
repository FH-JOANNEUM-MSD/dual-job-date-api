using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.DataAccess.Repositories;

public class AcademicDegreeRepository(AppDbContext dbContext)
    : BaseRepository<AcademicDegree>(dbContext), IAcademicDegreeRepository;