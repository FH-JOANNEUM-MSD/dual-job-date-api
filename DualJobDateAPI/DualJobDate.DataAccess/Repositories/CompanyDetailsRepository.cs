using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.DataAccess.Repositories;

public class CompanyDetailsRepository(AppDbContext dbContext)
    : BaseRepository<CompanyDetails>(dbContext), ICompanyDetailsRepository;