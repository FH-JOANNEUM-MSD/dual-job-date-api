using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DualJobDate.API.Controllers;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Resources;
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

        [Fact]
        public async Task GetCompany_ReturnsOkObjectResult_WithCompanyResource()
        {
            // Arrange
            var companyId = 1;
            var company = new Company { Id = companyId };
            var companyResource = new CompanyResource { Id = companyId };
            _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(companyId))
                .ReturnsAsync(company);
            _mapperMock.Setup(mapper => mapper.Map<Company, CompanyResource>(company))
                .Returns(companyResource);

            // Act
            var result = await _controller.GetCompany(companyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCompanyResource = Assert.IsType<CompanyResource>(okResult.Value);
            Assert.Equal(companyResource.Id, returnedCompanyResource.Id);
        }
        
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

            // Act
            var result = await _controller.GetCompanies(institutionId: 1, academicProgramId: null); //TODO: login first with a User

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<CompanyResource>>(okResult.Value);
            Assert.Equal(companies.Count, returnedCompanies.Count());
        }

        [Fact]
        public async Task GetCompanies_InvalidRequest_ReturnsBadRequestResult()
        {
            // Arrange
            var user = new User { /* Benutzerinformationen setzen */ };
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user); 

            // Act
            var result = await _controller.GetCompanies(institutionId: null, academicProgramId: null);  //TODO: login first with a User

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
            var user = new User { /* Benutzerinformationen setzen, z.B. AcademicProgramId */ };
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var companies = new List<Company> { /* Liste von aktiven Unternehmen erstellen */ };
            _companyServiceMock.Setup(service => service.GetActiveCompaniesAsync(user.AcademicProgramId)).ReturnsAsync(companies);

            // Act
            var result = await _controller.GetActiveCompanies();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<CompanyResource>>(okResult.Value);
            Assert.Equal(companies.Count, returnedCompanies.Count());
        }


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
            var res = result.Result;
            Assert.IsType<OkResult>(res);
        }




    }
