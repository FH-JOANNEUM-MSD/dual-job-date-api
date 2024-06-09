﻿using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.DataAccess.Repositories;

public class CompanyRepository(AppDbContext dbContext) : BaseRepository<Company>(dbContext), ICompanyRepository;