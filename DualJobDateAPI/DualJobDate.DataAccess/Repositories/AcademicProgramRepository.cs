using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.DataAccess.Repositories;

public class AcademicProgramRepository(AppDbContext dbContext) : BaseRepository<AcademicProgram>(dbContext), IAcademicProgramRepository
{
    public async Task<AcademicProgram> GetByName(string KeyName)
    {
        var result = await dbContext.Set<AcademicProgram>().Include(ap => ap.Activities).Include(co => co.Companies)
            .SingleOrDefaultAsync(i => i.KeyName == KeyName);
        return result;
    }
}