using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.DataAccess.Repositories;

public class InstitutionRepository(AppDbContext dbContext)
    : BaseRepository<Institution>(dbContext), IInstitutionRepository
{
    public async Task<Institution> GetByName(string KeyName)
    {
        var result = await dbContext.Set<Institution>()
            .SingleOrDefaultAsync(i => i.KeyName == KeyName);
        return result;
    }
}