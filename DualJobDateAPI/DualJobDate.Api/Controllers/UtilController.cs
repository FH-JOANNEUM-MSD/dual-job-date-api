using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UtilController(IUtilService utilService, IMapper mapper) : ControllerBase
{
    [Authorize("Admin")]
    [HttpGet("Institutions")]
    public async Task<ActionResult<IEnumerable<InstitutionResource>>> GetInstitutions()
    {
        var institutions = await utilService.GetInstitutionsAsync();
        var institutionResources = mapper.Map<IEnumerable<Institution>, IEnumerable<InstitutionResource>>(institutions);
        return Ok(institutionResources);
    }
    
    [Authorize("AdminOrInstitution")]
    [HttpGet("AcademicPrograms")]
    public async Task<ActionResult<IEnumerable<AcademicProgramResource>>> GetAcademicPrograms()
    {
        var academicPrograms = await utilService.GetAcademicProgramsAsync();
        var academicProgramResources = mapper.Map<IEnumerable<AcademicProgram>, IEnumerable<AcademicProgramResource>>(academicPrograms);
        return Ok(academicProgramResources);
    }
    
    [Authorize("AdminOrInstitution")]
    [HttpPost("AcademicProgram")]
    public async Task<ActionResult<AcademicProgramResource>> PostAcademicProgram([FromBody] AcademicProgramResource academicProgramResource)
    {
        var academicProgram = mapper.Map<AcademicProgramResource, AcademicProgram>(academicProgramResource);
        await utilService.PostAcademicProgramAsync(academicProgram);
        academicProgramResource = mapper.Map<AcademicProgram, AcademicProgramResource>(academicProgram);
        return Ok(academicProgramResource);
    }
    
    [Authorize("Admin")]
    [HttpPost("Institution")]
    public async Task<ActionResult<InstitutionResource>> PostInstitution([FromBody] InstitutionResource institutionResource)
    {
        var institution = mapper.Map<InstitutionResource, Institution>(institutionResource);
        await utilService.PostInstitutionAsync(institution);
        institutionResource = mapper.Map<Institution, InstitutionResource>(institution);
        return Ok(institutionResource);
    }
}