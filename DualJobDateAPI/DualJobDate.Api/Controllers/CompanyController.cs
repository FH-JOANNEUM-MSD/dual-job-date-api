using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class CompanyController(
    ICompanyService companyService,
    IMapper mapper,
    UserManager<User> userManager,
    IUtilService utilService,
    IPasswordGenerator generator,
    IUserStore<User> userStore,
    RoleManager<Role> roleManager)
    : ControllerBase
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();

    [HttpGet]
    public async Task<ActionResult<CompanyDto>> GetCompany([FromQuery] int id)
    {
        try
        {
            var company = await companyService.GetCompanyByIdAsync(id);
            var companyResource = mapper.Map<Company, CompanyDto>(company);
            return Ok(companyResource);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize("AdminOrInstitution")]
    [HttpGet("Companies")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies([FromQuery] int? institutionId,
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

        var companyResources = mapper.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(companies);
        return Ok(companyResources);
    }

    [Authorize(Policy = "Student")]
    [HttpGet("ActiveCompanies")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetActiveCompanies()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var companies = await companyService.GetActiveCompaniesAsync(user);
        var companyResources = mapper.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(companies);
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
    public async Task<ActionResult<CompanyDetailsDto>> GetCompanyDetails([FromQuery] int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var company = await companyService.GetCompanyByIdAsync(id);
        if (company == null) return NotFound("Company not found");

        var companyDetail = await companyService.GetCompanyDetailsAsync(company);
        var companyDetailResource = mapper.Map<CompanyDetails, CompanyDetailsDto>(companyDetail);
        return Ok(companyDetailResource);
    }

    [HttpGet("Activities")]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetCompanyActivities([FromQuery] int id)
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
    public async Task<IActionResult> UpdateCompanyActivities([FromBody] List<ActivityDto> resources)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        try
        {
            if (user.Company == null) return Unauthorized();

            var company = await companyService.GetCompanyByIdAsync(user.Company.Id);
            if (company == null) return NotFound("Company not found");

            var companyActivities =
                mapper.Map<IEnumerable<ActivityDto>, IEnumerable<CompanyActivity>>(resources);
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
    public async Task<ActionResult<CompanyDto>> AddCompany([FromBody] RegisterCompanyModel model)
    {
        var loginUser = await userManager.GetUserAsync(User);
        if (loginUser == null) return Unauthorized();

        var companyUser = await userManager.FindByEmailAsync(model.UserEmail);
        if (companyUser == null)
        {
            int institutionId;
            int programId = model.AcademicProgramId;

            if (User.IsInRole("Admin"))
            {
                if (!model.UserInstitutionId.HasValue)
                    return BadRequest("InstitutionId cannot be null!");

                institutionId = model.UserInstitutionId.Value;
            }
            else
            {
                institutionId = loginUser.InstitutionId;
            }

            var inst = utilService.GetInstitutionsAsync().Result.Where(x => x.Id == institutionId);
            if (inst == null) return NotFound("Institution not found");

            var ap = utilService.GetAcademicProgramsAsync(null).Result.Where(x => x.Id == programId);
            if (ap == null) return NotFound("AcademicProgram not found");

            if (string.IsNullOrEmpty(model.UserEmail) || !EmailAddressAttribute.IsValid(model.UserEmail))
                return BadRequest($"Email '{model.UserEmail}' is invalid.");

            var user = new User
            {
                Email = model.UserEmail,
                UserType = UserTypeEnum.Company,
                IsNew = true,
                InstitutionId = institutionId,
                AcademicProgramId = programId
            };

            await userStore.SetUserNameAsync(user, model.UserEmail, CancellationToken.None);
            var password = generator.GeneratePassword();

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var role = await roleManager.Roles.Where(r => r.UserTypeEnum == user.UserType).SingleOrDefaultAsync();
            if (role == null) return NotFound("Role doesn't exist");
            await userManager.AddToRoleAsync(user, role.Name);
            companyUser = user;
        }

        try
        {
            var company = await companyService.AddCompany(model.AcademicProgramId, model.CompanyName, companyUser);
            var companyResource = mapper.Map<Company, CompanyDto>(company);
            return Ok(companyResource);
        }
        catch (ArgumentOutOfRangeException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize("Company")]
    [HttpPost("Locations")]
    public async Task<IActionResult> AddLocations([FromBody] List<AddressDto> resources)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        try
        {
            var company = await companyService.GetCompanyByUser(user);
            if (company == null) return Unauthorized();

            var addresses = mapper.Map<IEnumerable<AddressDto>, IEnumerable<Address>>(resources);
            await companyService.AddLocations(addresses, company);
            return Ok();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("Locations")]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetLocations([FromQuery] int? companyId)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        Company company;
        if (User.IsInRole("Company"))
        {
            company = await companyService.GetCompanyByUser(user);
        }
        else
        {
            company = await companyService.GetCompanyByIdAsync((int)companyId);
        }
        if (company == null) return NotFound("Company not found");

        var locations = await companyService.GetLocationsByCompanyAsync(company);
        var locationResources = mapper.Map<IEnumerable<Address>, IEnumerable<AddressDto>>(locations);
        return Ok(locationResources);
    }

}