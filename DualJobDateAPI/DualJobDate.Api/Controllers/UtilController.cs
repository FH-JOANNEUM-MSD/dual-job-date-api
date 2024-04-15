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
}