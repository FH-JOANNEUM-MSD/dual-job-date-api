﻿using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers
{
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
        [HttpGet("/Companies")]
        public async Task<ActionResult<IEnumerable<CompanyResource>>> GetCompanies([FromQuery] int? institutionId,
            [FromQuery] int? academicProgramId)
        {

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var companies = new List<Company>();
            if (User.IsInRole("Admin") && institutionId.HasValue)
            {
                companies = await companyService.GetCompaniesByInstitutionAsync((int)institutionId);
            }
            else if (academicProgramId.HasValue)
            {
                companies = await companyService.GetCompaniesByAcademicProgramAsync((int)academicProgramId);
            }
            else
            {
                return BadRequest("Invalid request parameters or insufficient permissions.");
            }

            var companyResources = mapper.Map<IEnumerable<Company>, IEnumerable<CompanyResource>>(companies);
            return Ok(companyResources);
        }

        [Authorize(Policy = "Student")]
        [HttpGet("GetActiveCompanies")]
        public async Task<ActionResult<IEnumerable<CompanyResource>>> GetActiveCompanies()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var companies = await companyService.GetActiveCompaniesAsync(user.AcademicProgramId);
            var companyResources = mapper.Map<IEnumerable<Company>, IEnumerable<CompanyResource>>(companies);
            return Ok(companyResources);
        }

        [Authorize(Policy = "Company")]
        [HttpPost()]
        public async Task<ActionResult<Company>> UpdateCompany(UpdateCompanyModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                await companyService.UpdateCompany(model, user.Company);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = "Company")]
        [HttpPost("/IsActive")]
        public async Task<IActionResult> UpdateCompanyActivity([FromQuery] int? companyId, [FromQuery] bool isActive)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.CompanyId != null)
            {
                companyId = user.CompanyId;
            }

            try
            {
                if (companyId == null)
                    throw new Exception("CompanyId is null");
                var company = await companyService.GetCompanyByIdAsync((int)companyId);
                await companyService.UpdateCompanyActivity(isActive, company);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("/Details")]
        public async Task<ActionResult<CompanyDetailsResource>> GetCompanyDetails([FromQuery] int? companyId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.CompanyId != null)
            {
                companyId = user.CompanyId;
            }

            if (companyId == null)
                throw new Exception("CompanyId is null");
            var company = await companyService.GetCompanyByIdAsync((int)companyId);
            if (company == null)
            {
                return NotFound("Company not found");
            }

            var companyDetail = await companyService.GetCompanyDetailsAsync(company);
            var companyDetailResource = mapper.Map<CompanyDetails, CompanyDetailsResource>(companyDetail);
            return Ok(companyDetailResource);
        }

        [Authorize("Company")]
        [HttpPost("/Details")]
        public async Task<IActionResult> AddOrUpdateCompanyDetails(CompanyDetailsResource resource)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                var companyDetails = mapper.Map<CompanyDetailsResource, CompanyDetails>(resource);
                if (user.CompanyId != null)
                {
                    var company = await companyService.GetCompanyByIdAsync((int)user.CompanyId);
                    await companyService.UpdateCompanyDetails(companyDetails, company);
                    return Ok();
                }
                else
                {
                    return NotFound("Company not found");
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("/Activities")]
        public async Task<ActionResult<IEnumerable<ActivityResource>>> GetCompanyActivities([FromQuery] int? companyId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.CompanyId != null)
            {
                companyId = user.CompanyId;
            }

            if (companyId == null)
                throw new Exception("CompanyId is null");
            var company = await companyService.GetCompanyByIdAsync((int)companyId);
            if (company == null)
            {
                return NotFound("Company not found");
            }

            var companyActivities = await companyService.GetCompanyActivitiesAsync(company);
            return Ok(companyActivities);
        }

        [Authorize("Company")]
        [HttpPost("/Activities")]
        public async Task<IActionResult> UpdateCompanyActivities([FromBody] List<ActivityResource> resources)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                if (user.CompanyId == null)
                {
                    return NotFound("Company not found");

                }

                var company = await companyService.GetCompanyByIdAsync((int)user.CompanyId);
                if (company == null)
                {
                    return NotFound("Company not found");
                }

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
        [HttpPost("/Register")]
        public async Task<ActionResult<CompanyResource>> AddCompany([FromBody] RegisterCompanyModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                var company = await companyService.AddCompany(model.AcademicProgramId, model.CompanyName);
                var companyResource = mapper.Map<Company, CompanyResource>(company);
                return Ok(companyResource);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
    
}
