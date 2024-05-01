using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Dtos;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UtilController(IUtilService utilService, IMapper mapper) : ControllerBase
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
    public async Task<ActionResult<IEnumerable<AcademicProgramDto>>> GetAcademicProgramsByInstitution([FromQuery] int id)
    {
        var academicPrograms = await utilService.GetAcademicProgramsAsync().Result.Where(x => x.InstitutionId ==id).ToListAsync();
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
}