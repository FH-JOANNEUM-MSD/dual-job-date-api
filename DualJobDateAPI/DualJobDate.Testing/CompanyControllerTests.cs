using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DualJobDate.API.Controllers;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DualJobDate.Testing;


    public class CompanyControllerTests
    {
        private readonly CompanyController _controller;
        private readonly Mock<ICompanyService> _companyServiceMock = new Mock<ICompanyService>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<UserManager<User>> _userManagerMock = MockHelpers.MockUserManager<User>();
        private readonly Mock<User> _user = new Mock<User>();
    

        public CompanyControllerTests()
        {
            _controller = new CompanyController(
                _companyServiceMock.Object,
                _mapperMock.Object,
                _userManagerMock.Object
            );
        }
        
        // Tests for GetCompany 
        [Fact]
        public async Task GetCompany_ReturnsOkObjectResult_WithCompanyResource() 
        {
            // Arrange
            var companyId = 1;
            var company = new Company { Id = companyId };
            var companyResource = new CompanyDto { Id = companyId };
            _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(companyId))
                .ReturnsAsync(company);
            _mapperMock.Setup(mapper => mapper.Map<Company, CompanyDto>(company))
                .Returns(companyResource);

            // Act
            var result = await _controller.GetCompany(companyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCompanyResource = Assert.IsType<CompanyDto>(okResult.Value);
            Assert.Equal(companyResource.Id, returnedCompanyResource.Id);
        }
        
        // Tests for GetCompanies
        [Fact]
        public async Task GetCompanies_UnauthorizedUser_ReturnsUnauthorizedResult() 
        {
            // Arrange
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetCompanies(institutionId: 1, academicProgramId: 1);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }
        
        [Fact]
             public async Task GetCompanies_ValidRequest_ReturnsOkResult() 
             {
                 // Arrange
                 var user = new User { /* Benutzerinformationen setzen */ };
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var companies = new List<Company> { /* Firmenliste erstellen */ };
            _companyServiceMock.Setup(service => service.GetCompaniesByInstitutionAsync(It.IsAny<int>())).ReturnsAsync(companies);
            
            //_userManagerMock.Setup(it => it.User.IsInRole("Admin"))
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

            // Act
            var result = await _controller.GetCompanies(institutionId: 1, academicProgramId: 1); 

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<CompanyDto>>(okResult.Value);
            Assert.Equal(companies.Count, returnedCompanies.Count());
        }

        [Fact]
        public async Task GetCompanies_InvalidRequest_ReturnsBadRequestResult() 
        {
            // Arrange
            var user = new User { /* Benutzerinformationen setzen */ };
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
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


        // Act
        var result = await _controller.GetCompanies(institutionId: null, academicProgramId: null);  //TODO: login first with a User

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // Tests for GetActiveCompanies
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
            var user = new User { /* Benutzerinformationen setzen, z.B. AcademicProgramId */ };
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var companies = new List<Company> { /* Liste von aktiven Unternehmen erstellen */ };
            _companyServiceMock.Setup(service => service.GetActiveCompaniesAsync(user)).ReturnsAsync(companies);

            // Act
            var result = await _controller.GetActiveCompanies();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<CompanyDto>>(okResult.Value);
            Assert.Equal(companies.Count, returnedCompanies.Count());
        }

        // Tests for UpdateCompanies
        [Fact]
        public async Task PutCompany_SuccessfulUpdate_ReturnsOkResult() 
        {
            // Arrange
            var companyId = 1;
            var updatedModel = new UpdateCompanyModel { Name = "Test747"/* aktualisierte Unternehmensdaten */ };
            var company = new Company { Id = companyId };
    
            // Mock-Setup für GetCompanyByUser
            _userManagerMock.Setup(service => service.GetUserAsync(_controller.User)).ReturnsAsync(_user.Object);
            _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(companyId)).ReturnsAsync(company);
            _companyServiceMock.Setup(service => service.GetCompanyByUser(_user.Object)).ReturnsAsync(company);

            // Act
            var result = await _controller.UpdateCompany(updatedModel);

            // Assert
            var res = result;
            Assert.IsType<OkResult>(res);
        } 
        
        // Tests for Update Company Activity
        
        [Fact]
        public async Task UpdateCompanyActivity_UnauthorizedUser_ReturnsUnauthorizedResult()
        {
            // Arrange
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

            // Act
            var result = await _controller.UpdateCompanyActivity(id: 1, isActive: true);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task UpdateCompanyActivity_CompanyNotFound_ReturnsBadRequestResult() 
        {
            // Arrange
            var user = new User { /* Benutzerinformationen setzen */ };
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _companyServiceMock.Setup(service => service.GetCompanyByUser(user)).ReturnsAsync((Company)null);

            _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(1)).ThrowsAsync(new Exception("Company not found"));
            _companyServiceMock.Setup(service => service.UpdateCompanyActivity(It.IsAny<bool>(), It.IsAny<Company>()))
                .ThrowsAsync(new Exception("Company not found"));
            

            // Act
            var result = await _controller.UpdateCompanyActivity(id: 1, isActive: true);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            // Weitere Überprüfung, ob die BadRequest-Meldung korrekt ist
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Company not found", badRequestResult.Value);
        }



        [Fact]
        public async Task UpdateCompanyActivity_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var user = new User { /* Benutzerinformationen setzen */ };
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var company = new Company { Id = 1 };
            _companyServiceMock.Setup(service => service.GetCompanyByUser(user)).ReturnsAsync(company);
            _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _controller.UpdateCompanyActivity(id: 1, isActive: true);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        // Tests for AddOrUpdateCompanyDetails
/*
        [Fact]
        public async Task AddOrUpdateCompanyDetails_ValidDetails_ReturnsOkResult()
        {
            // Arrange
            var user = new User { /* Benutzerinformationen setzen */ /*};
            var resource = new CompanyDetailsDto { ShortDescription = "Kurze Beschreibung des Unternehmens"/* weitere Unternehmensdetails */ /*};
            var company = new Company {Name = "Magna", Id = 1 };
            user.Company = company;
    
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _companyServiceMock.Setup(service => service.GetCompanyByUser(user)).ReturnsAsync(company);
            _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(user.Company.Id)).ReturnsAsync(company);
            _companyServiceMock.Setup(service => service.UpdateCompanyDetails(It.IsAny<CompanyDetails>(), It.IsAny<Company>())).Verifiable();
            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailsDto, CompanyDetails>(It.IsAny<CompanyDetailsDto>()))
                .Returns((CompanyDetailsDto resource) =>
                {
                    // new CompanyDetails-Objekt 
                    return new CompanyDetails
                    {
                        ShortDescription = resource.ShortDescription,
                        TeamPictureBase64 = resource.TeamPictureBase64,
                        JobDescription = resource.JobDescription,
                        ContactPersonInCompany = resource.ContactPersonInCompany,
                    };
                });

            // Act
            var result = await _controller.AddOrUpdateCompanyDetails(resource);

            // Assert
            Assert.IsType<OkResult>(result);
            
            _companyServiceMock.Verify(service => service.UpdateCompanyDetails(It.IsAny<CompanyDetails>(), It.IsAny<Company>()), Times.Once);
        }
*/
        /*
        [Fact]
        public async Task AddOrUpdateCompanyDetails_UpdateFailed_ReturnsNotFoundResult()
        {
            // Arrange
            var user = new User {  };
            var resource = new CompanyDetailsDto { ShortDescription = "Kurze Beschreibung des Unternehmens"  };
            var company = new Company { Name = "Magna", Id = 1 };
            user.Company = company;

            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _companyServiceMock.Setup(service => service.GetCompanyByUser(user)).ReturnsAsync(company);
            _companyServiceMock.Setup(service => service.UpdateCompanyDetails(It.IsAny<CompanyDetails>(), It.IsAny<Company>()))
                .ThrowsAsync(new Exception("Update failed"));
            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailsDto, CompanyDetails>(It.IsAny<CompanyDetailsDto>()))
                .Returns((CompanyDetailsDto resource) =>
                {
                    return new CompanyDetails
                    {
                        ShortDescription = resource.ShortDescription,
                        TeamPictureBase64 = resource.TeamPictureBase64,
                        JobDescription = resource.JobDescription,
                        ContactPersonInCompany = resource.ContactPersonInCompany,
                    };
                });

            // Act
            var result = await _controller.AddOrUpdateCompanyDetails(resource);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }*/
        
        //GetCompany Activities
        
        [Fact]
        public async Task GetCompanyActivities_ValidId_ReturnsOkResult()
        {
            // Arrange
            var userId = "user1";
            var companyId = 1;
            var user = new User { Id = userId, /* Weitere Benutzerinformationen */ };
            var company = new Company { Id = companyId, /* Weitere Unternehmensdetails */ };
            
            var activities = new List<Activity>
            {
                new Activity { Id = 1, Name = "Activity 1", /* Weitere Eigenschaften */ },
                new Activity { Id = 2, Name = "Activity 2", /* Weitere Eigenschaften */ },
            };
            
            // converts the Activity List into ActivityResource-Objects
            var activityResources = activities.Select(activity => new ActivityDto
            {
                Id = activity.Id,
                Name = activity.Name,
                
            }).ToList();
            
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(companyId)).ReturnsAsync(company);
            _companyServiceMock.Setup(service => service.GetCompanyActivitiesAsync(company)).ReturnsAsync(activityResources);

            // Act
            var result = await _controller.GetCompanyActivities(companyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedActivities = Assert.IsAssignableFrom<IEnumerable<ActivityDto>>(okResult.Value);
            Assert.Equal(activityResources.Count, returnedActivities.Count());
        }
    
        [Fact]
        public async Task GetCompanyActivities_InvalidId_ReturnsBadRequestResult()
        {
            // Arrange
            var invalidCompanyId = -1; // ungültige Unternehmens-ID
            var user = new User { /* Benutzerinformationen */ };
            var company = new Company { Id = invalidCompanyId, /* Weitere Unternehmensdetails */ };

            var activities = new List<Activity>
            {
                new Activity { Id = 1, Name = "Activity 1", /* Weitere Eigenschaften */ },
                new Activity { Id = 2, Name = "Activity 2", /* Weitere Eigenschaften */ },
            };

            // converts the Activity List into ActivityResource-Objects
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

        



    }

