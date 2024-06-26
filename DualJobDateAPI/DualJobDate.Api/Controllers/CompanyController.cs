﻿using System.ComponentModel.DataAnnotations;
using AutoMapper;
using DualJobDate.Api.Controllers;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DualJobDate.BusinessLogic.Exceptions;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class CompanyController(
    ICompanyService companyService,
    RoleManager<Role> roleManager, 
    IServiceProvider serviceProvider,
    IMapper mapper, 
    UserManager<User> userManager,    
    IUtilService utilService,
    IUnitOfWork unitOfWork)
    : ControllerBase
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<CompanyDto>> GetCompany([FromQuery] int? id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            throw new UserNotFoundException();

        var company = User.IsInRole(UserTypeEnum.Company.ToString())
            ? await companyService.GetCompanyByUser(user)
            : await companyService.GetCompanyByIdAsync(id ?? throw new CompanyNotFoundException(id));
        
        if (company is null)
            throw new CompanyNotFoundException(id);

        var companyResource = mapper.Map<Company, CompanyDto>(company);
        return Ok(companyResource);
    }

    [Authorize("AdminOrInstitution")]
    [HttpGet("Companies")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies(
        [FromQuery] int? institutionId,
        [FromQuery] int? academicProgramId)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            throw new UserNotFoundException();

        if (!academicProgramId.HasValue & !institutionId.HasValue)
            throw new InvalidParametersException();

        var companies = new List<Company>();
        if (User.IsInRole(UserTypeEnum.Admin.ToString()) && institutionId.HasValue)
            companies = await companyService.GetCompaniesByInstitutionAsync(institutionId.Value);

        if (academicProgramId.HasValue)
            companies = await companyService.GetCompaniesByAcademicProgramAsync(academicProgramId.Value);

        var companyResources = mapper.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(companies);
        return Ok(companyResources);
    }

    [Authorize(Policy = "Student")]
    [HttpGet("ActiveCompanies")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetActiveCompanies()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
            throw new UserNotFoundException();

        var companies = await companyService.GetActiveCompaniesAsync(user);
        var companyResources = mapper.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(companies);
        return Ok(companyResources);
    }

    [Authorize(Policy = "Company")]
    [HttpPut("UpdateCompany")]
    public async Task<IActionResult> UpdateCompany(UpdateCompanyModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            throw new UserNotFoundException();

        var company = await companyService.GetCompanyByUser(user);
        if (company is null)
            throw new CompanyNotFoundException();
        
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
            TrainerPosition = model.TrainerPosition,
            Addresses = model.Addresses
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

    [Authorize(Policy = "WebApp")]
    [HttpPut("IsActive")]
    public async Task<IActionResult> UpdateCompanyActivity([FromQuery] int? id, [FromQuery] bool isActive)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            throw new UserNotFoundException();

        var company = User.IsInRole(UserTypeEnum.Company.ToString())
            ? await companyService.GetCompanyByUser(user)
            : await companyService.GetCompanyByIdAsync(id ?? throw new CompanyNotFoundException(id));
        
        if (company is null)
            throw new CompanyNotFoundException(id);

        await companyService.UpdateCompanyActivity(isActive, company);
        return Ok();
    }

    [Authorize]
    [HttpGet("Details")]
    public async Task<ActionResult<CompanyDetailsDto>> GetCompanyDetails([FromQuery] int? id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            throw new UserNotFoundException();

        var company = User.IsInRole(UserTypeEnum.Company.ToString())
            ? await companyService.GetCompanyByUser(user)
            : await companyService.GetCompanyByIdAsync(id ?? throw new CompanyNotFoundException(id));
        
        if (company is null)
            throw new CompanyNotFoundException(id);

        var companyDetail = await companyService.GetCompanyDetailsAsync(company);
        var companyDetailResource = mapper.Map<CompanyDetails, CompanyDetailsDto>(companyDetail);
        return Ok(companyDetailResource);
    }

    [Authorize]
    [HttpGet("Activities")]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetCompanyActivities([FromQuery] int? id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            throw new UserNotFoundException();

        var company = User.IsInRole(UserTypeEnum.Company.ToString())
            ? await companyService.GetCompanyByUser(user)
            : await companyService.GetCompanyByIdAsync(id ?? throw new CompanyNotFoundException(id));
        
        if (company is null)
            throw new CompanyNotFoundException(id);

        var companyActivities = await companyService.GetCompanyActivitiesAsync(company);
        return Ok(companyActivities);
    }

    [Authorize("Company")]
    [HttpPost("Activities")]
    public async Task<IActionResult> UpdateCompanyActivities([FromBody] List<ActivityDto> resources)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            throw new UserNotFoundException();

        var company = await companyService.GetCompanyByUser(user);
        
        if (company is null)
            throw new CompanyNotFoundException();

        var companyActivities =
            mapper.Map<IEnumerable<ActivityDto>, IEnumerable<CompanyActivity>>(resources);
        await companyService.UpdateCompanyActivities(companyActivities, user.Company);
        return Ok();
    }

    [Authorize("AdminOrInstitution")]
    [HttpPost("Register")]
    public async Task<ActionResult<CompanyDto>> AddCompany([FromBody] RegisterCompanyModel model)
    {
        var adminUser = await userManager.GetUserAsync(User);
        if (adminUser is null)
            throw new UserNotFoundException();
        
        if (model.AcademicProgramId == null)
            return BadRequest("InstitutionId or AcademicProgramId cannot be null!");
        int program = (int)model.AcademicProgramId;
        
        var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();

        if (string.IsNullOrEmpty(model.UserEmail) || !EmailAddressAttribute.IsValid(model.UserEmail))
            return BadRequest($"Email '{model.UserEmail}' is invalid.");

        var academicProgram = await utilService.GetAcademicProgramsAsync(null).Result
            .Where(ac => ac.Id == model.AcademicProgramId).FirstOrDefaultAsync();       

        var user = new User
        {
            Email = model.UserEmail,
            UserType = UserTypeEnum.Company,
            IsNew = true,
            InstitutionId = academicProgram.InstitutionId,
            AcademicProgramId = program
        };

        await userStore.SetUserNameAsync(user, model.UserEmail, CancellationToken.None);

        var password = UserController.GeneratePassword();

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded) return BadRequest(result.Errors);
        var role = await roleManager.Roles.Where(r => r.UserTypeEnum == UserTypeEnum.Company).SingleOrDefaultAsync();
        if (role == null) return NotFound("Role doesn't exist");
        await userManager.AddToRoleAsync(user, role.Name);
        

        var company = await companyService.AddCompany(model.AcademicProgramId, model.CompanyName, user);
        if (company is null)
            throw new CompanyNotFoundException();

        var activites = await utilService.GetActivitiesByAcademicProgramAsync(model.AcademicProgramId);
        var companyActivites = new List<CompanyActivity>();

        foreach (var activity in activites)
        {

            companyActivites.Add(new CompanyActivity()
            {
                ActivityId = activity.Id,
                CompanyId = company.Id,
                Value = 0
            });
        }
        
        await unitOfWork.CompanyActivityRepository.AddRangeAsync(companyActivites);

        var companyResource = mapper.Map<Company, CompanyDto>(company);
        return Ok(companyResource);
    }
    
    // [Authorize("Company")]
    // [HttpPost("Locations")]
    // public async Task<IActionResult> AddLocations([FromBody] List<AddressDto> resources)
    // {
    //     var user = await userManager.GetUserAsync(User);
    //     if (user is null)
    //         throw new UserNotFoundException();
    //
    //
    //     var company = await companyService.GetCompanyByUser(user);
    //     if (company is null)
    //         throw new CompanyNotFoundException();
    //
    //     var addresses = mapper.Map<IEnumerable<AddressDto>, IEnumerable<Address>>(resources);
    //     await companyService.AddLocations(addresses, company);
    //     return Ok();
    // }
    //
    // [Authorize]
    // [HttpGet("Locations")]
    // public async Task<ActionResult<IEnumerable<AddressDto>>> GetLocations([FromQuery] int? companyId)
    // {
    //     var user = await userManager.GetUserAsync(User);
    //     if (user is null)
    //         throw new UserNotFoundException();
    //
    //     var company = User.IsInRole(UserTypeEnum.Company.ToString())
    //         ? await companyService.GetCompanyByUser(user)
    //         : await companyService.GetCompanyByIdAsync(companyId ?? throw new CompanyNotFoundException(companyId));
    //
    //     if (company is null)
    //         throw new CompanyNotFoundException(companyId);
    //
    //     var locations = await companyService.GetLocationsByCompanyAsync(company);
    //     var locationResources = mapper.Map<IEnumerable<Address>, IEnumerable<AddressDto>>(locations);
    //     return Ok(locationResources);
    // }
}