﻿using DualJobDate.BusinessObjects.Dto;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using System.Text.Json;

namespace DualJobDate.BusinessLogic.Services
{
    public class CompanyService : ICompanyService
    {
        public Task<string> GetCompanyByIdAsync(int id)
        {
            var result = JsonSerializer.Serialize(new CompanyDto(id, "Foo Bar Company"));
            return Task.FromResult(result);
        }
    }
}