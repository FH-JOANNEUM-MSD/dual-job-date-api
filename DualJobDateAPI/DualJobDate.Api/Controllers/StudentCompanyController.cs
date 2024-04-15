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
    public async Task<ActionResult> AddStudentCompany([FromQuery] bool like, int companyId)
    {
        var company = await companyService.GetCompanyByIdAsync(companyId);
        if (company == null)
        {
            return NotFound();
        }
        
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        if (studentCompanyService.AddStudentCompany(like, companyId, user.Id) == null)
        {
            return BadRequest();
        }

        return Ok();
    }
}