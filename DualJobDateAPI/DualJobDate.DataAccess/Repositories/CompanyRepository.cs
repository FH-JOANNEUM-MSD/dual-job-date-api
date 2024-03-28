using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.DataAccess.Repositories;

public class CompanyRepository(AppDbContext dbContext) : BaseRepository<Company>(dbContext), ICompanyRepository;