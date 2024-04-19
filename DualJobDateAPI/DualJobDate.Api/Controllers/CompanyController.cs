using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class CompanyController(ICompanyService companyService, IMapper mapper, UserManager<User> userManager)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CompanyResource>> GetCompany([FromQuery] int id)
    {
        try
        {
            var company = await companyService.GetCompanyByIdAsync(id) 
                ?? throw new Exception($"The requested company with ID [{id}] could not be found.");
            var companyResource = mapper.Map<Company, CompanyResource>(company);
            return Ok(companyResource);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize("AdminOrInstitution")]
    [HttpGet("Companies")]
    public async Task<ActionResult<IEnumerable<CompanyResource>>> GetCompanies(
        [FromQuery] int? institutionId,
        [FromQuery] int? academicProgramId)
    {
        List<Company> companies;
        if (User.IsInRole("Admin") && institutionId.HasValue)
            companies = await companyService.GetCompaniesByInstitutionAsync(institutionId.Value);
        else if (academicProgramId.HasValue)
            companies = await companyService.GetCompaniesByAcademicProgramAsync(academicProgramId.Value);
        else
            return BadRequest("Invalid request parameters or insufficient permissions.");

        var companyResources = mapper.Map<IEnumerable<Company>, IEnumerable<CompanyResource>>(companies);
        return Ok(companyResources);
    }

    [Authorize(Policy = "Student")]
    [HttpGet("ActiveCompanies")]
    public async Task<ActionResult<IEnumerable<CompanyResource>>> GetActiveCompanies()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var companies = await companyService.GetActiveCompaniesAsync(user.AcademicProgramId);
        var companyResources = mapper.Map<IEnumerable<Company>, IEnumerable<CompanyResource>>(companies);
        return Ok(companyResources);
    }

    [Authorize(Policy = "Company")]
    [HttpPut("UpdateCompany")]
    public async Task<IActionResult> UpdateCompany(UpdateCompanyModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var company = await companyService.GetCompanyByUser(user);
        if (company is null)
            return NotFound("Company not found");

        var companyDetails = new CompanyDetails
        { 
            ShortDescription = model.ShortDescription,
            TeamPictureBase64 = model.TeamPictureBase64,
            JobDescription = model.JobDescription,
            ContactPersonInCompany = model.ContactPersonInCompany,
            ContactPersonHRM = model.ContactPersonHRM,
            Trainer = model.Trainer,
            TrainerTraining = model.TrainerTraining,
            TrainerProfessionalExperience = model.TrainerProfessionalExperience,
            TrainerPosition = model.TrainerPosition
        };

        try
        {
            await companyService.UpdateCompany(model, company);
            await companyService.UpdateCompanyDetails(companyDetails, company);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Policy = "Company")]
    [HttpPut("IsActive")]
    public async Task<IActionResult> UpdateCompanyActivity([FromQuery] bool isActive, [FromQuery] int? companyId)
    {
        Company? company = null;
        if (companyId.HasValue)
            company = await companyService.GetCompanyByIdAsync(companyId.Value);

        if (company is null)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            company = await companyService.GetCompanyByUser(user);
        }

        if (company is null)
            return NotFound("Company not found");

        try
        {
            await companyService.UpdateCompanyActivity(isActive, company);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("Details")]
    public async Task<ActionResult<CompanyDetailsResource>> GetCompanyDetails([FromQuery] int id)
    {
        var company = await companyService.GetCompanyByIdAsync(id);
        if (company is null)
            return NotFound("Company not found");

        var companyDetail = await companyService.GetCompanyDetailsAsync(company);
        if (companyDetail is null)
            return NotFound("CompanyDetail not found");

        var companyDetailResource = mapper.Map<CompanyDetails, CompanyDetailsResource>(companyDetail);
        return Ok(companyDetailResource);
    }

    [HttpGet("Activities")]
    public async Task<ActionResult<IEnumerable<ActivityResource>>> GetCompanyActivities([FromQuery] int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var company = await companyService.GetCompanyByIdAsync(id);
        if (company is null)
            return NotFound("Company not found");

        var companyActivities = await companyService.GetCompanyActivitiesAsync(company);
        return Ok(companyActivities);
    }

    [Authorize("Company")]
    [HttpPost("Activities")]
    public async Task<IActionResult> UpdateCompanyActivities([FromBody] List<ActivityResource> resources)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        user.Company ??= await companyService.GetCompanyByUser(user);

        if (user.Company is null)
            return NotFound("Company not found");

        try
        {
            var companyActivities =
                mapper.Map<IEnumerable<ActivityResource>, IEnumerable<CompanyActivity>>(resources);
            await companyService.UpdateCompanyActivities(companyActivities, user.Company);
            return Ok();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize("AdminOrInstitution")]
    [HttpPost("Register")]
    public async Task<ActionResult<CompanyResource>> AddCompany([FromBody] RegisterCompanyModel model)
    {
        var companyUser = await userManager.FindByEmailAsync(model.UserEmail);
        if (companyUser is null)
            return NotFound("User not found");

        try
        {
            var company = await companyService.AddCompany(model.AcademicProgramId, model.CompanyName, companyUser);
            if (company is null)
                return NotFound("Company not found");

            var companyResource = mapper.Map<Company, CompanyResource>(company);
            return Ok(companyResource);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}