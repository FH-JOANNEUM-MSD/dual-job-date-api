using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class CompanyController(ICompanyService myService)
        : ControllerBase
    {
        [HttpGet("{id}")]
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
}
