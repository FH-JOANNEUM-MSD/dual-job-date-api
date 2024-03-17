using DualJobDate.BusinessObjects.Dto;

namespace DualJobDate.BusinessLogic.Services.Interface;

public interface ICompanyService
{
    Task<string> GetCompanyByIdAsync(int id);
}
