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
            var company = await companyService.GetCompanyByIdAsync(id);
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
    public async Task<ActionResult<IEnumerable<CompanyResource>>> GetCompanies([FromQuery] int? institutionId,
        [FromQuery] int? academicProgramId)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var companies = new List<Company>();
        if (User.IsInRole("Admin") && institutionId.HasValue)
            companies = await companyService.GetCompaniesByInstitutionAsync((int)institutionId);
        else if (academicProgramId.HasValue)
            companies = await companyService.GetCompaniesByAcademicProgramAsync((int)academicProgramId);
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
        if (user == null) return Unauthorized();

        var companies = await companyService.GetActiveCompaniesAsync(user.AcademicProgramId);
        var companyResources = mapper.Map<IEnumerable<Company>, IEnumerable<CompanyResource>>(companies);
        return Ok(companyResources);
    }

    [Authorize(Policy = "Company")]
    [HttpPut("UpdateCompany")]
    public async Task<IActionResult> UpdateCompany(UpdateCompanyModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var company = await companyService.GetCompanyByUser(user);
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
        if (company == null) return NotFound("Company not found");
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

    [Authorize(Policy = "WebApp")]
    [HttpPut("IsActive")]
    public async Task<IActionResult> UpdateCompanyActivity([FromQuery] int id, [FromQuery] bool isActive)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var userCompany = await companyService.GetCompanyByUser(user);
        if (userCompany != null) id = userCompany.Id;

        try
        {
            if (id == null)
                throw new Exception("CompanyId is null");
            var company = await companyService.GetCompanyByIdAsync(id);
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
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var company = await companyService.GetCompanyByIdAsync(id);
        if (company == null) return NotFound("Company not found");

        var companyDetail = await companyService.GetCompanyDetailsAsync(company);
        var companyDetailResource = mapper.Map<CompanyDetails, CompanyDetailsResource>(companyDetail);
        return Ok(companyDetailResource);
    }

    [HttpGet("Activities")]
    public async Task<ActionResult<IEnumerable<ActivityResource>>> GetCompanyActivities([FromQuery] int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        var company = await companyService.GetCompanyByIdAsync(id);
        if (company == null) return NotFound("Company not found");

        var companyActivities = await companyService.GetCompanyActivitiesAsync(company);
        return Ok(companyActivities);
    }

    [Authorize("Company")]
    [HttpPost("Activities")]
    public async Task<IActionResult> UpdateCompanyActivities([FromBody] List<ActivityResource> resources)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        try
        {
            if (user.Company == null) return Unauthorized();

            var company = await companyService.GetCompanyByIdAsync(user.Company.Id);
            if (company == null) return NotFound("Company not found");

            var companyActivities =
                mapper.Map<IEnumerable<ActivityResource>, IEnumerable<CompanyActivity>>(resources);
            await companyService.UpdateCompanyActivities(companyActivities, company);
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
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var companyUser = await userManager.FindByEmailAsync(model.UserEmail);
        if (companyUser == null) return NotFound("User not found");
        try
        {
            var company = await companyService.AddCompany(model.AcademicProgramId, model.CompanyName, companyUser);
            var companyResource = mapper.Map<Company, CompanyResource>(company);
            return Ok(companyResource);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}