using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Dtos;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UtilController(UserManager<User> userManager, IUtilService utilService, IMapper mapper) : ControllerBase
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
    public async Task<ActionResult<IEnumerable<AcademicProgramDto>>> GetAcademicProgramsByInstitution([FromQuery] int? id)
    {
        var user = userManager.GetUserAsync(User);
        List<AcademicProgram> academicPrograms;
        if (User.IsInRole("Institution"))
        {
            id = user.Result.InstitutionId;
            academicPrograms = await utilService.GetAcademicProgramsAsync().Result.Where(x => x.InstitutionId == id).ToListAsync();
        }
        else
        {
            if (id is null)
            {
                academicPrograms = await utilService.GetAcademicProgramsAsync().Result.ToListAsync();
            }
            academicPrograms = await utilService.GetAcademicProgramsAsync().Result.Where(x => x.InstitutionId == id).ToListAsync();
        }
        var academicProgramResources = mapper.Map<IEnumerable<AcademicProgram>, IEnumerable<AcademicProgramDto>>(academicPrograms);
        return Ok(academicProgramResources);
    }
    
    [Authorize("AdminOrInstitution")]
    [HttpPost("AcademicProgram")]
    public async Task<ActionResult<AcademicProgramDto>> PostAcademicProgram([FromQuery] int? institutionId, [FromBody] AcademicProgramModel model)
    {
        var user = userManager.GetUserAsync(User);
        if (User.IsInRole("Institution"))
        {
            institutionId = user.Result.InstitutionId;
        }
        else
        {
            if (institutionId == null)
            {
                return BadRequest("InstitutionId is mandatory!");
            }
        }
        try
        {
            var academicProgram = await utilService.PostAcademicProgramAsync((int)institutionId, model);
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
}