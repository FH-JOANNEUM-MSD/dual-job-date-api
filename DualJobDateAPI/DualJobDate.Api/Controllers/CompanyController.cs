using DualJobDate.BusinessLogic.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers;

public class CompanyController(ICompanyService myService) : ControllerBase
{
    [HttpGet("[controller]/{id}")]
    public async Task<IActionResult> Index(int id)
    {
        try
        {
            var company = await myService.GetCompanyByIdAsync(id);
            return Ok(company);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
