using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController(ICompanyService myService, IMapper mapper, UserManager<User> userManager)
        : ControllerBase
    {
        [HttpGet("{companyId}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            try
            {
                var company = await myService.GetCompanyByIdAsync(id);
                return Ok(company);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/Institution/{companyId}")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompaniesByInstitution(int id)
        {
            try
            {
                var companies = await myService.GetCompaniesByInstitution(id);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        [HttpGet("/AcademicProgram/{companyId}")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompaniesByAcademicProgram(int id)
        {
            try
            {
                var companies = await myService.GetCompaniesByAcademicProgram(id);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
        [HttpPost()]
        public async Task<ActionResult<Company>> UpdateCompany(UpdateCompanyModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            try
            {
                await myService.UpdateCompany(model, user.Company);                
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        [HttpPost("/IsActive")]
        public async Task<ActionResult<Company>> UpdateCompanyActivity(bool isActive, int? companyId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.Company != null)
            {
                try
                {
                    await myService.UpdateCompanyActivity(isActive, user.Company);                
                    return Ok();
                }
                catch (Exception e)
                {
                    return NotFound(e.Message);
                }
            }
            else
            {
                try
                {
                    if (companyId == null)
                        throw new Exception("CompanyId is null");
                    var company = await myService.GetCompanyByIdAsync((int) companyId);
                    await myService.UpdateCompanyActivity(isActive, user.Company);                
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
        }
        
        
        [HttpPost("/Details")]
        public async Task<ActionResult<Company>> UpdateCompanyDetails(CompanyDetailsResource resource)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            try
            {
                var companyDetails = mapper.Map<CompanyDetailsResource, CompanyDetails>(resource);
                await myService.UpdateCompanyDetails(companyDetails, user.Company);                
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
