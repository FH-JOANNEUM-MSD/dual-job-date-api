using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DualJobDate.BusinessObjects.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class StudentCompanyController(UserManager<User> userManager, IUtilService utilService, ICompanyService companyService, IStudentCompanyService studentCompanyService, IMapper mapper) : ControllerBase
{
    [Authorize(Policy = "AdminOrInstitution")]
    [HttpGet("GetLikesAndDislikes")]
    public async Task<ActionResult<List<StudentCompanyDto>>> GetStudentCompanies(int academicProgramId)
    {
        var user = await userManager.GetUserAsync(User);
        var ap = await utilService.GetAcademicProgramsAsync().Result.Where(ap => ap.Id == academicProgramId).SingleOrDefaultAsync();
        if (User.IsInRole("Institution") && user.InstitutionId != ap.InstitutionId)
        {
            return Unauthorized("No permission to see Likes from this Academic Program");
        }
        if (user == null)
        {
            return Unauthorized();
        }
        
        var studentCompanies = await studentCompanyService.GetStudentCompaniesAsync(academicProgramId);
        var studentCompanyDtoList = mapper.Map<List<StudentCompany>, List<StudentCompanyDto>>(studentCompanies);
        return Ok(studentCompanyDtoList);
    }
    
    [Authorize(Policy = "AdminOrInstitutionOrStudent")]
    [HttpGet("GetLikesAndDislikesByStudentId")]
    public async Task<ActionResult<List<StudentCompanyDto>>> GetStudentCompaniesByStudentId([FromQuery] string? studentId)
    {
        var user = await userManager.GetUserAsync(User);
        var student = await userManager.Users.Include(u => u.AcademicProgram).Where(u => u.Id == studentId).SingleOrDefaultAsync();
        if (User.IsInRole("Institution") && user.InstitutionId != student.AcademicProgram.InstitutionId)
        {
            return Unauthorized("No permission to see Likes from this Academic Program");
        }
        if (user == null)
        {
            return Unauthorized();
        }
        
        if (User.IsInRole("Student"))
        {
            studentId = user.Id;
        }
        
        var studentCompanies = await studentCompanyService.GetStudentCompaniesByStudentIdAsync(studentId);
        var studentCompanyDtoList = mapper.Map<List<StudentCompany>, List<StudentCompanyDto>>(studentCompanies);
        return Ok(studentCompanyDtoList);
    }
    
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

        if (studentCompanyService.CreateStudentCompanyAsync(like, companyId, user.Id) == null)
        {
            return StatusCode(500, "Adding like or dislike failed.");
        }

        return Ok("Adding like or dislike succeeded.");
    }
    
    [Authorize(Policy = "Student")]
    [HttpDelete("RemoveLikeOrDislike")]
    public async Task<ActionResult> DeleteStudentCompany([FromQuery] int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var studentCompany = await studentCompanyService.GetStudentCompanyByIdAsync(id);
        if (studentCompany != null && studentCompany.StudentId != user.Id)
        {
            return BadRequest("A student can't delete the likes and dislikes of another student.");
        }
        
        if (await studentCompanyService.DeleteStudentCompanyAsync(id))
        {
            return Ok("Removing like or dislike succeeded.");
        }

        return StatusCode(500, "Removing like or dislike failed.");
    }
}