using AutoMapper;
using DualJobDate.BusinessLogic.Exceptions;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Dtos;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UtilController(IUtilService utilService, IMapper mapper, UserManager<User> userManager) : ControllerBase
{
    [Authorize("Admin")]
    [HttpGet("Institutions")]
    public async Task<ActionResult<IEnumerable<InstitutionDto>>> GetInstitutions()
    {
        var institutions = await utilService.GetInstitutionsAsync();
        var institutionResources = mapper.Map<IEnumerable<Institution>, IEnumerable<InstitutionDto>>(institutions);
        return Ok(institutionResources);
    }
    
    [Authorize("AdminOrInstitution")]
    [HttpGet("AcademicPrograms")]
    public async Task<ActionResult<IEnumerable<AcademicProgramDto>>> GetAcademicPrograms(int? institutionId)
    {
        var academicPrograms = await utilService.GetAcademicProgramsAsync(institutionId);
        var academicProgramResources = mapper.Map<IEnumerable<AcademicProgram>, IEnumerable<AcademicProgramDto>>(academicPrograms);
        return Ok(academicProgramResources);
    }
    
    [Authorize("AdminOrInstitution")]
    [HttpPost("AcademicProgram")]
    public async Task<ActionResult<AcademicProgramDto>> PostAcademicProgram([FromQuery] int id, [FromBody] AcademicProgramModel model)
    {
        try
        {
            var academicProgram = await utilService.PostAcademicProgramAsync(id, model);
            var academicProgramResource = mapper.Map<AcademicProgram, AcademicProgramDto>(academicProgram);
            return Ok(academicProgramResource);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    
    [Authorize("Admin")]
    [HttpPost("Institution")]
    public async Task<ActionResult<InstitutionDto>> PostInstitution([FromBody] InstitutionModel model)
    {
        try
        {
            var institution = await utilService.PostInstitutionAsync(model);
            var institutionResource = mapper.Map<Institution, InstitutionDto>(institution);
            return Ok(institutionResource);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred");
        }
    }
    
    [HttpGet("AppointmentsByUserId")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByUserIdAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        try
        {
            var appointments = await utilService.GetAppointmentsByUserIdAsync(user.Id);
            var appointmentResources = mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDto>>(appointments);
            return Ok(appointmentResources);
        }
        catch (Exception ex)
        {
            throw new AppointmentNotFoundException("user");
        }
    }
    
    [Authorize("Admin")]
    [HttpGet("AppointmentsByUserId/{userId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByUserIdAsync(string userId)
    {
        try
        {
            var appointments = await utilService.GetAppointmentsByUserIdAsync(userId);
            var appointmentResources = mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDto>>(appointments);
            return Ok(appointmentResources);
        }
        catch (Exception ex)
        {
            throw new AppointmentNotFoundException(userId);
        }
    }
    
    [Authorize("Admin")]
    [HttpGet("AppointmentsByCompanyId")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByCompanyIdAsync(int companyId)
    {
        try
        {
            var appointments = await utilService.GetAppointmentsByCompanyIdAsync(companyId);
            var appointmentResources = mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDto>>(appointments);
            return Ok(appointmentResources);
        }
        catch (Exception ex)
        {
            throw new AppointmentNotFoundException($"Company: {companyId}");
        }
    }
    
    [HttpGet("TestTermine")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetTestTermine()
    {
        var appointments = await utilService.GetAppointmentsByCompanyIdAsync(1);
        var newApp = new Appointment
        {
            AppointmentDate = DateTime.Now,
            CompanyId = 2,
            UserId = "8e778201-2c4e-4243-91eb-1eb8b895f004"
        };
        var newApp2 = new Appointment
        {
            AppointmentDate = DateTime.Now.Add(TimeSpan.FromDays(1)),
            CompanyId = 1,
            UserId = "8e778201-2c4e-4243-91eb-1eb8b895f004"
        };
        appointments.Add(newApp);
        appointments.Add(newApp2);
        var appointmentResources = mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDto>>(appointments);
        return Ok(appointmentResources);
    }
}