using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class StudentCompanyController(UserManager<User> userManager, ICompanyService companyService, IStudentCompanyService studentCompanyService) : ControllerBase
{
    [Authorize(Policy = "Student")]
    [HttpPost("AddLikeOrDislike")]
    public async Task<ActionResult> CreateStudentCompany([FromQuery] bool like, int companyId)
    {
        var company = await companyService.GetCompanyByIdAsync(companyId);
        if (company == null)
        {
            return NotFound("Company not found.");
        }
        
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        if (studentCompanyService.CreateStudentCompany(like, companyId, user.Id) == null)
        {
            return BadRequest("Adding like or dislike failed.");
        }

        return Ok("Adding like or dislike succeeded.");
    }
    
    [Authorize(Policy = "Student")]
    [HttpDelete("RemoveLikeOrDislike")]
    public async Task<ActionResult> DeleteStudentCompany([FromQuery] int id)
    {
        if (await studentCompanyService.DeleteStudentCompany(id))
        {
            return Ok("Removing like or dislike succeeded.");
        }

        return BadRequest("Removing like or dislike failed.");
    }
}