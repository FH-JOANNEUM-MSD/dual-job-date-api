using System.Security.Claims;
using AutoMapper;
using DualJobDate.API.Controllers;
using DualJobDate.Api.Mapping;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DualJobDate.Testing;

public class CompanyControllerTests
{
    private readonly CompanyController _controller;
    private readonly Mock<ICompanyService> _companyServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<User> _user;
    private IMapper? mapperR;
    
    public CompanyControllerTests()
    {
        _companyServiceMock = new Mock<ICompanyService>();
        _mapperMock = new Mock<IMapper>();
        _userManagerMock = MockHelpers.MockUserManager<User>();
        _user = new Mock<User>();
        _controller = new CompanyController(
            _companyServiceMock.Object,
            _mapperMock.Object,
            _userManagerMock.Object
        );
        SetupMapper();
    }

    [Fact]
    public async Task GetCompany_ReturnsOkObjectResult_WithCompanyResource() 
    {
        // Arrange
        var company = new Company { Id = 3 };
        var companyResource = new CompanyDto { Id = company.Id };
        _companyServiceMock.Setup(service =>
                service.GetCompanyByIdAsync(company.Id)).ReturnsAsync(company);
        _mapperMock.Setup(mapper =>
                mapper.Map<Company, CompanyDto>(company))
            .Returns(companyResource);

        // Act
        var result = await _controller.GetCompany(company.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCompanyResource = Assert.IsType<CompanyDto>(okResult.Value);
        Assert.Equal(companyResource.Id, returnedCompanyResource.Id);
    }
    
    [Fact]
    public async Task GetCompanies_UnauthorizedUser_ReturnsUnauthorizedResult() 
    {
        // Arrange
        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

        // Act
        var result = await _controller.GetCompanies(1, 1);

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }
    
    [Fact]
    public async Task GetCompanies_ValidRequest_ReturnsOkResult() 
    { 
        // Arrange
        var user = TestHelper.GetTestAdminUser();
        
        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var companies = new List<Company>
        {
            new() {Id = 1, InstitutionId = 2, AcademicProgramId = 1 }
        };
        _companyServiceMock.Setup(service =>
            service.GetCompaniesByAcademicProgramAsync(It.IsAny<int>())).ReturnsAsync(companies);
        _mapperMock.Setup(mapper =>
            mapper.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(companies))
            .Returns(mapperR!.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(companies));
        
        MockClaimUser(user);

        // Act
        var result = await _controller.GetCompanies(2, 1); 

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<CompanyDto>>(okResult.Value).ToList();
        Assert.Equal(companies.Count, returnedCompanies.Count);
        Assert.True(returnedCompanies.All(rc => companies.Any(c => c.Id == rc.Id)) &&
                    companies.All(c => returnedCompanies.Any(rc => rc.Id == c.Id)));
    }

    [Fact]
    public async Task GetCompanies_InvalidRequest_ReturnsBadRequestResult() 
    {
        // Arrange
        var user = TestHelper.GetTestAdminUser();
        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        MockClaimUser(user);

        // Act
        var result = await _controller.GetCompanies(institutionId: null, academicProgramId: null);  

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task GetActiveCompanies_UserNotAuthenticated_ReturnsUnauthorizedResult() 
    {
        // Arrange
        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

        // Act
        var result = await _controller.GetActiveCompanies();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetActiveCompanies_ValidRequest_ReturnsOkResultWithCompanies() 
    {
        // Arrange
        var user = TestHelper.GetTestAdminUser();
        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var companies = new List<Company> { TestHelper.GetTestCompany() };
        _companyServiceMock.Setup(service =>
            service.GetActiveCompaniesAsync(user)).ReturnsAsync(companies);
        _mapperMock.Setup(mapper =>
            mapper.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(companies))
            .Returns(mapperR!.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(companies));

        // Act
        var result = await _controller.GetActiveCompanies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<CompanyDto>>(okResult.Value).ToList();
        Assert.Equal(companies.Count, returnedCompanies.Count);
        Assert.True(returnedCompanies.All(rc => companies.Any(c => c.Id == rc.Id)) &&
                    companies.All(c => returnedCompanies.Any(rc => rc.Id == c.Id)));
    }

    [Fact]
    public async Task PutCompany_SuccessfulUpdate_ReturnsOkResult() 
    {
        // Arrange
        var updatedModel = new UpdateCompanyModel { Name = "Test747" };
        var company = new Company { Id = 1 };
        _userManagerMock.Setup(service =>
            service.GetUserAsync(_controller.User)).ReturnsAsync(_user.Object);
        _companyServiceMock.Setup(service =>
            service.GetCompanyByIdAsync(company.Id)).ReturnsAsync(company);
        _companyServiceMock.Setup(service =>
            service.GetCompanyByUser(_user.Object)).ReturnsAsync(company);

        // Act
        var result = await _controller.UpdateCompany(updatedModel);

        // Assert
        Assert.IsType<OkResult>(result);
    } 
    
    [Fact]
    public async Task UpdateCompanyActivity_UnauthorizedUser_ReturnsUnauthorizedResult()
    {
        // Arrange
        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

        // Act
        var result = await _controller.UpdateCompanyActivity(id: 1, isActive: true);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task UpdateCompanyActivity_CompanyNotFound_ReturnsBadRequestResult() 
    {
        // Arrange
        var user = new User();
        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _companyServiceMock.Setup(service =>
            service.GetCompanyByUser(user)).ReturnsAsync((Company)null);

        _companyServiceMock.Setup(service =>
            service.GetCompanyByIdAsync(1)).ThrowsAsync(new Exception("Company not found"));
        _companyServiceMock.Setup(service =>
                service.UpdateCompanyActivity(It.IsAny<bool>(), It.IsAny<Company>()))
            .ThrowsAsync(new Exception("Company not found"));
        

        // Act
        var result = await _controller.UpdateCompanyActivity(id: 1, isActive: true);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.Equal("Company not found", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateCompanyActivity_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var user = new User();
        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var company = new Company { Id = 1 };
        _companyServiceMock.Setup(service => service.GetCompanyByUser(user)).ReturnsAsync(company);
        _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(1)).ReturnsAsync(company);

        // Act
        var result = await _controller.UpdateCompanyActivity(id: 1, isActive: true);

        // Assert
        Assert.IsType<OkResult>(result);
    }
    
    [Fact]
    public async Task GetCompanyActivities_ValidId_ReturnsOkResult()
    {
        // Arrange
        var user = new User { Id = "98aad5f7-1fdc-45a1-9015-fbbfaf79e351" };
        var company = new Company { Id = 1 };
        
        var activities = new List<Activity>
        {
            new() { Id = 1, Name = "Activity 1"},
            new() { Id = 2, Name = "Activity 2"}
        };
        
        var activityResources = activities.Select(activity => new ActivityDto
        {
            Id = activity.Id,
            Name = activity.Name,
            
        }).ToList();
        
        _userManagerMock.Setup(manager =>manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(company.Id)).ReturnsAsync(company);
        _companyServiceMock.Setup(service => service.GetCompanyActivitiesAsync(company)).ReturnsAsync(activityResources);

        // Act
        var result = await _controller.GetCompanyActivities(company.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedActivities = Assert.IsAssignableFrom<IEnumerable<ActivityDto>>(okResult.Value);
        Assert.Equal(activityResources.Count, returnedActivities.Count());
    }

    [Fact]
    public async Task GetCompanyActivities_InvalidId_ReturnsBadRequestResult()
    {
        // Arrange
        const int invalidCompanyId = -1;
        var user = new User();
        var company = new Company { Id = invalidCompanyId};

        var activities = new List<Activity>
        {
            new() { Id = 1, Name = "Activity 1" },
            new() { Id = 2, Name = "Activity 2" },
        };

        var activityResources = activities.Select(activity => new ActivityDto
        {
            Id = activity.Id,
            Name = activity.Name,
        }).ToList();

        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(invalidCompanyId)).ReturnsAsync((Company)null);
        _companyServiceMock.Setup(service => service.GetCompanyActivitiesAsync(company)).ReturnsAsync(activityResources);

        // Act
        var result = await _controller.GetCompanyActivities(invalidCompanyId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Company not found", notFoundResult.Value);
    }
    
     [Fact]
    public async Task GetCompanyDetails_ValidId_ReturnsCompanyDetails()
    {
        // Arrange
        var user = TestHelper.GetTestAdminUser();
        var company = new Company { Id = 1 };
        var companyDetails = new CompanyDetails();
        var companyDetailsDto = new CompanyDetailsDto();

        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(company.Id)).ReturnsAsync(company);
        _companyServiceMock.Setup(service => service.GetCompanyDetailsAsync(company)).ReturnsAsync(companyDetails);
        _mapperMock.Setup(mapper => mapper.Map<CompanyDetails, CompanyDetailsDto>(companyDetails)).Returns(companyDetailsDto);

        // Act
        var result = await _controller.GetCompanyDetails(company.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var resultDto = Assert.IsType<CompanyDetailsDto>(okResult.Value);
        Assert.Equal(companyDetailsDto, resultDto);
    }

    [Fact]
    public async Task GetCompanyDetails_UnauthorizedUser_ReturnsUnauthorizedResult()
    {
        // Arrange
        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

        // Act
        var result = await _controller.GetCompanyDetails(1);

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetCompanyDetails_InvalidId_ReturnsNotFoundResult()
    {
        // Arrange
        var companyId = -1; 
        var user = new User( );

        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(companyId)).ReturnsAsync((Company)null);

        // Act
        var result = await _controller.GetCompanyDetails(companyId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task UpdateCompanyActivities_ValidData_ReturnsOkResult()
    {
        // Arrange
        var user = new User();
        var company = new Company { Name = "Magna", Id = 1 };
        user.Company = company;
        var activityDtos = new List<ActivityDto>();
        

        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(user.Company.Id)).ReturnsAsync(company);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ActivityDto>, IEnumerable<CompanyActivity>>(activityDtos))
            .Returns(new List<CompanyActivity>()); 

        // Act
        var result = await _controller.UpdateCompanyActivities(activityDtos);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateCompanyActivities_UnauthorizedUser_ReturnsUnauthorizedResult()
    {
        // Arrange
        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

        // Act
        var result = await _controller.UpdateCompanyActivities([]);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task UpdateCompanyActivities_InvalidCompany_ReturnsNotFoundResult()
    {
        // Arrange
        var user = new User();
        var company = new Company();
        user.Company = company;

        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(user.Company.Id)).ReturnsAsync((Company)null);

        // Act
        var result = await _controller.UpdateCompanyActivities([]);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
  
    
    [Fact]
    public async Task AddCompany_ValidData_ReturnsOkResult()
    {
        // Arrange
        var user = new User { };
        var companyUser = new User { };
        var company = new Company { };
        var model = new RegisterCompanyModel
        {
            AcademicProgramId = 2, 
            CompanyName = "Example Company", 
            UserEmail = "example@example.com" 
        };

        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(manager => manager.FindByEmailAsync(model.UserEmail)).ReturnsAsync(companyUser);
        _companyServiceMock.Setup(service => service.AddCompany(model.AcademicProgramId, model.CompanyName, companyUser)).ReturnsAsync(company);
        _mapperMock.Setup(mapper => mapper.Map<Company, CompanyDto>(company)).Returns(new CompanyDto()); // Rückgabe von Dummy CompanyDto für den Test

        // Act
        var result = await _controller.AddCompany(model);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task AddCompany_UnauthorizedUser_ReturnsUnauthorizedResult()
    {
        // Arrange
        var model = new RegisterCompanyModel
        {
            AcademicProgramId = 2, 
            CompanyName = "Example Company", 
            UserEmail = "example@example.com" 
        };
        
        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

        // Act
        var result = await _controller.AddCompany(model);

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task AddCompany_UserNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var user = new User { };
        var model = new RegisterCompanyModel
        {
            AcademicProgramId = 2,
            CompanyName = "Example Company", 
            UserEmail = "example@example.com" 
        };
        
        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(manager => manager.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        // Act
        var result = await _controller.AddCompany(model);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }


    private void SetupMapper()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ModelToResourceProfile());
            mc.AddProfile(new ResourceToModelProfile());
        });

        mapperR = mappingConfig.CreateMapper();
    }

    private void MockClaimUser(User user)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            ], "mock"))
        };
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };
    }
}