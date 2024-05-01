using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.DataAccess.Repositories;

public class AcademicProgramRepository(AppDbContext dbContext)
    : BaseRepository<AcademicProgram>(dbContext), IAcademicProgramRepository
{
    public async Task<AcademicProgram> GetByNameAndYear(string KeyName, int Year)
    {
        var result = await dbContext.Set<AcademicProgram>().Include(ap => ap.Activities)
            .SingleOrDefaultAsync(ap => ap.KeyName == KeyName && ap.Year == Year);
        return result;
    }
}