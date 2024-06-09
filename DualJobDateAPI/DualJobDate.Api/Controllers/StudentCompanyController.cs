using AutoMapper;
using DualJobDate.BusinessLogic.Exceptions;
using DualJobDate.BusinessObjects.Dtos;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Engines;

namespace DualJobDate.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class StudentCompanyController(UserManager<User> userManager, ICompanyService companyService, IStudentCompanyService studentCompanyService, IMapper mapper) : ControllerBase
{
    [Authorize(Policy = "AdminOrInstitution")]
    [HttpGet("GetLikesAndDislikes")]
    public async Task<ActionResult<List<StudentCompanyDto>>> GetStudentCompanies()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        
        var studentCompanies = await studentCompanyService.GetStudentCompaniesAsync();
        var studentCompanyDtoList = mapper.Map<List<StudentCompany>, List<StudentCompanyDto>>(studentCompanies);
        return Ok(studentCompanyDtoList);
    }
    
    [Authorize(Policy = "AdminOrInstitutionOrStudent")]
    [HttpGet("GetLikesAndDislikesByStudentId")]
    public async Task<ActionResult<List<StudentCompanyDto>>> GetStudentCompaniesByStudentId([FromQuery] string? studentId)
    {
        var user = await userManager.GetUserAsync(User);
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
    
    // public async Task<ActionResult> Match([FromBody]  MatchModel model)
    // {
    //     var user = await userManager.GetUserAsync(User);
    //     if (user is null)
    //     {
    //         throw new UserNotFoundException();
    //     }
    //
    //     var students = userManager.Users.Where(x =>
    //         x.InstitutionId == user.InstitutionId && x.AcademicProgramId == model.AcademicProgramId && x.IsActive &&
    //         x.UserType == UserTypeEnum.Student).ToList();
    //     
    //     var companies = companyService.GetCompaniesByAcademicProgramAsync(model.AcademicProgramId).Result
    //         .Where(company => company.IsActive).ToList();
    //     
    //     var appointments = await studentCompanyService.GetMatches(model, students, companies);
    // }

    [Authorize(Policy = "AdminOrInstitution")]
    [HttpGet("MatchCompaniesToStudent")]
    public async Task<ActionResult> MatchCompaniesToStudent([FromQuery] MatchModel model)
    {
        
        var companies = companyService.GetCompaniesByAcademicProgramAsync(model.AcademicProgramId).Result
            .Where(company => company.IsActive).ToList();        
        var students = userManager.Users.Where(x => x.AcademicProgramId == model.AcademicProgramId &&
                                                    x.UserType == UserTypeEnum.Student).ToList();

        var appointments = new List<Appointment>();
        var matches = studentCompanyService.MatchCompaniesToStudents(students, companies);
        var duration = (model.EndTime - model.StartTime) / model.MatchesPerStudent;
        foreach (var match in matches)
        {
            for (int i = 0; i < match.Value.Count; i++)
            {
                appointments.Add(new Appointment
                {
                    StartTime = model.StartTime + duration * i,
                    EndTime = model.StartTime + duration * (i + 1),
                    User = match.Key,
                    Company = match.Value[i],
                });
            }
        }

        await studentCompanyService.DeleteAppointments(model.AcademicProgramId);
        
        await studentCompanyService.SaveAppointments(appointments);
        return Ok();
    }
}