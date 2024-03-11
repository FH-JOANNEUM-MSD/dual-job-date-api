using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.DataAccess;

namespace DualJobDate.DataAccess.Repositories
{
    public class InstitutionRepository(AppDbContext dbContext)
        : BaseRepository<Institution>(dbContext), IInstitutionRepository;
}
