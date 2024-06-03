using System.Security.Claims;
using AutoMapper;
using DualJobDate.API.Controllers;
using DualJobDate.Api.Mapping;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;
using DualJobDate.BusinessObjects.Entities.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using DualJobDate.BusinessLogic.Exceptions;
using DualJobDate.BusinessLogic.Services;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DualJobDate.Testing.Controller;

public class CompanyControllerTests
{
    private readonly CompanyController _controller;
    private readonly Mock<ICompanyService> _companyServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IUtilService> _utilServiceMock;
    private readonly Mock<User> _user;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private IMapper? mapper;

    public CompanyControllerTests()
    {
        _companyServiceMock = new Mock<ICompanyService>();
        _mapperMock = new Mock<IMapper>();
        _userManagerMock = MockHelpers.MockUserManager<User>();
        _user = new Mock<User>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _utilServiceMock = new Mock<IUtilService>();
        var mockRoleManager = MockHelpers.MockRoleManager<Role>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _controller = new CompanyController(
            _companyServiceMock.Object,
            mockRoleManager.Object,
            _serviceProviderMock.Object,
            _mapperMock.Object,
            _userManagerMock.Object,
            _utilServiceMock.Object,
            _unitOfWorkMock.Object
        );
        SetupMapper();
    }

    [Fact]
    public async Task GetCompany_ReturnCompanyNotFoundException()
    {
        // Arrange
        var user = TestHelper.GetTestUser(1, 1, UserTypeEnum.Admin);

        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        MockClaimUser(user);
        async Task Act() => await _controller.GetCompany(0);
        await Assert.ThrowsAsync<CompanyNotFoundException>(Act);
    }

    [Fact]
    public async Task GetCompany_ReturnsOkObjectResult_WithCompanyResource()
    {
        // Arrange
        var user = TestHelper.GetTestUser(1, 1, UserTypeEnum.Admin);

        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        MockClaimUser(user);
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
    public async Task GetCompanies_Exception()
    {
        // Arrange
        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

        // Act
        async Task Act() => await _controller.GetCompanies(1, 1);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }

    [Theory]
    [InlineData(1, 1, UserTypeEnum.Admin, new[] { 1, 2 })]
    [InlineData(2, 1, UserTypeEnum.Student, new[] { 3 })]
    [InlineData(2, 2, UserTypeEnum.Company, new[] { 4 })]
    public async Task GetCompaniesValidRequestReturnsOkResult(int institutionId, int academicProgramId, UserTypeEnum userType, int[] expectedIds)
    {
        // Arrange
        var user = TestHelper.GetTestUser(institutionId, academicProgramId, userType);

        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var companies = new List<Company>
        {
            new() {Id = 1, InstitutionId = 1, AcademicProgramId = 1 },
            new() {Id = 2, InstitutionId = 1, AcademicProgramId = 2 },
            new() {Id = 3, InstitutionId = 2, AcademicProgramId = 1 },
            new() {Id = 4, InstitutionId = 2, AcademicProgramId = 2 },
        };

        var expectedCompanies = companies.Where(x => expectedIds.Contains(x.Id)).ToList();

        _companyServiceMock.Setup(service =>
            service.GetCompaniesByAcademicProgramAsync(It.IsAny<int>())).ReturnsAsync(expectedCompanies);

        _companyServiceMock.Setup(service =>
            service.GetCompaniesByInstitutionAsync(It.IsAny<int>())).ReturnsAsync(expectedCompanies);

        _mapperMock.Setup(mapper =>
            mapper.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(expectedCompanies))
            .Returns(mapper!.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(expectedCompanies));

        MockClaimUser(user);

        // Act
        var result = await _controller.GetCompanies(institutionId, academicProgramId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<CompanyDto>>(okResult.Value).ToList();
        Assert.Equal(expectedCompanies.Count, returnedCompanies.Count);
        Assert.True(returnedCompanies.All(rc => expectedCompanies.Any(c => c.Id == rc.Id)) &&
                    expectedCompanies.All(c => returnedCompanies.Any(rc => rc.Id == c.Id)));
    }

    [Fact]
    public async Task GetCompanies_InvalidRequest_ReturnsBadRequestResult()
    {
        // Arrange
        var user = TestHelper.GetTestUser();
        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        MockClaimUser(user);

        // Act
        async Task Act() => await _controller.GetCompanies(null, null);

        // Assert
        await Assert.ThrowsAsync<InvalidParametersException>(Act);
    }

    [Fact]
    public async Task GetActiveCompanies_UserNotAuthenticated_ReturnsUnauthorizedResult()
    {
        // Arrange
        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

        // Act
        async Task Act() => await _controller.GetActiveCompanies();

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }

    [Fact]
    public async Task GetActiveCompanies_ValidRequest_ReturnsOkResultWithCompanies()
    {
        // Arrange
        var user = TestHelper.GetTestUser();
        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var companies = new List<Company> { TestHelper.GetTestCompany() };
        _companyServiceMock.Setup(service =>
            service.GetActiveCompaniesAsync(user)).ReturnsAsync(companies);
        _mapperMock.Setup(mapper =>
            mapper.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(companies))
            .Returns(mapper!.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(companies));

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
        async Task Act() => await _controller.UpdateCompanyActivity(1, true);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }

    [Fact]
    public async Task UpdateCompanyActivity_CompanyNotFound_ReturnsBadRequestResult()
    {
        //TODO Write Test
    }

    [Theory]
    [InlineData(UserTypeEnum.Admin)]
    [InlineData(UserTypeEnum.Institution)]
    [InlineData(UserTypeEnum.Company)]
    public async Task UpdateCompanyActivity_ValidRequest_ReturnsOkResult(UserTypeEnum userType)
    {
        // Arrange
        var user = TestHelper.GetTestUser(1, 1, userType);

        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        MockClaimUser(user);

        var companyOfUser = new Company { Id = 1, UserId = user.Id};
        var companyOfNotUser = new Company { Id = 2, UserId = "NoUserId" };
        _companyServiceMock.Setup(service => service.GetCompanyByUser(user)).ReturnsAsync(companyOfUser);
        _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(1)).ReturnsAsync(companyOfNotUser);

        // Act
        var result = await _controller.UpdateCompanyActivity(id: 1, isActive: true);

        // Assert
        if (userType == UserTypeEnum.Admin || userType == UserTypeEnum.Institution)
            _companyServiceMock.Verify(service => service.GetCompanyByIdAsync(1), Times.Once);
        else
            _companyServiceMock.Verify(service => service.GetCompanyByUser(user), Times.Once);
        
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetCompanyActivities_ValidId_ReturnsOkResult()
    {
        // Arrange
        var user = TestHelper.GetTestUser(1, 1, UserTypeEnum.Admin);

        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        MockClaimUser(user);
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

        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
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
        var user = TestHelper.GetTestUser(1, 1, UserTypeEnum.Admin);

        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        MockClaimUser(user);

        // Arrange
        const int invalidCompanyId = -1;
        var company = new Company { Id = invalidCompanyId };

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

        _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(invalidCompanyId)).ReturnsAsync((Company)null);
        _companyServiceMock.Setup(service => service.GetCompanyActivitiesAsync(company)).ReturnsAsync(activityResources);

        // Act
        async Task Act() => await _controller.GetCompanyActivities(invalidCompanyId);

        // Assert
        await Assert.ThrowsAsync<CompanyNotFoundException>(Act);
    }

    [Fact]
    public async Task GetCompanyDetails_ValidId_ReturnsCompanyDetails()
    {
        // Arrange
        var user = TestHelper.GetTestUser(1, 1, UserTypeEnum.Admin);

        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        MockClaimUser(user);        var company = new Company { Id = 1 };
        var companyDetails = new CompanyDetails();
        var companyDetailsDto = new CompanyDetailsDto();

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
        async Task Act() => await _controller.GetCompanyDetails(1);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }

    [Fact]
    public async Task GetCompanyDetails_InvalidId_ReturnsNotFoundResult()
    {
        // Arrange
        var user = TestHelper.GetTestUser(1, 1, UserTypeEnum.Admin);

        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        MockClaimUser(user);
        // Arrange
        var companyId = -1;

        _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(companyId)).ReturnsAsync((Company)null);

        // Act
        async Task Act() => await _controller.GetCompanyDetails(companyId);

        // Assert
        await Assert.ThrowsAsync<CompanyNotFoundException>(Act);
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
        async Task Act() => await _controller.UpdateCompanyActivities([]);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }

    [Fact]
    public async Task UpdateCompanyActivities_InvalidCompany_ReturnsNotFoundResult()
    {
        // Arrange
        var user = TestHelper.GetTestUser(1, 1, UserTypeEnum.Admin);
        user.Company = null;
        MockClaimUser(user);

        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        // Act
        async Task Act() => await _controller.UpdateCompanyActivities([]);

        // Assert
        await Assert.ThrowsAsync<CompanyNotFoundException>(Act);
    }


    [Fact]
    public async Task AddCompany_ValidData_ReturnsOkResult()
    {
        // Arrange
        var user = new User { };
        var company = new Company { Id = 1 };
        var model = new RegisterCompanyModel
        {
            AcademicProgramId = 2,
            CompanyName = "Example Company",
            UserEmail = "example@example.com"
        };

        _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _companyServiceMock.Setup(service => service.AddCompany(model.AcademicProgramId, model.CompanyName, user)).ReturnsAsync(company);
        _mapperMock.Setup(mapper => mapper.Map<Company, CompanyDto>(company)).Returns(new CompanyDto());

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
        async Task Act() => await _controller.AddCompany(model);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
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
        async Task Act() => await _controller.AddCompany(model);

        // Assert
        await Assert.ThrowsAsync<CompanyNotFoundException>(Act);
    }


    private void SetupMapper()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ModelToResourceProfile());
            mc.AddProfile(new ResourceToModelProfile());
        });

        mapper = mappingConfig.CreateMapper();
    }

    private void MockClaimUser(User user)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            ], "mock"))
        };
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };
    }
}