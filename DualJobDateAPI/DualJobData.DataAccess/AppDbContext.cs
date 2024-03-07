using Microsoft.EntityFrameworkCore;

namespace DualJobData.DataAccess
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IDisposable
    {
        //TODO DbSets
        //TODO OnConfiguring

        void IDisposable.Dispose() => base.Dispose();
    }
}
