using Microsoft.AspNetCore.Http;

namespace DualJobDate.Testing.Controller;

using System.Security.Claims;
using AutoMapper;
using DualJobDate.Api.Controllers;
using DualJobDate.BusinessObjects.Dtos;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class StudentCompanyControllerTests
{
    [Fact]
    public async Task GetStudentCompanies_ReturnsListOfStudentCompanies()
    {
        // Arrange
        var userManagerMock = MockHelpers.MockUserManager<User>();
        var companyServiceMock = new Mock<ICompanyService>();
        var studentCompanyServiceMock = new Mock<IStudentCompanyService>();
        var mapperMock = new Mock<IMapper>();

        var user = new User();
        var studentCompanies = new List<StudentCompany> { new StudentCompany() };
        var studentCompanyDtoList = new List<StudentCompanyDto> { new StudentCompanyDto() };

        userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        studentCompanyServiceMock.Setup(service => service.GetStudentCompaniesAsync()).ReturnsAsync(studentCompanies);
        mapperMock.Setup(mapper => mapper.Map<List<StudentCompany>, List<StudentCompanyDto>>(studentCompanies))
            .Returns(studentCompanyDtoList);

        var controller = new StudentCompanyController(userManagerMock.Object, companyServiceMock.Object,
            studentCompanyServiceMock.Object, mapperMock.Object);

        // Act
        var result = await controller.GetStudentCompanies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedStudentCompanies = Assert.IsAssignableFrom<List<StudentCompanyDto>>(okResult.Value);
        Assert.Equal(studentCompanyDtoList.Count, returnedStudentCompanies.Count);
    }
    
    [Fact]
    public async Task GetStudentCompaniesByStudentId_ReturnsListOfStudentCompaniesFilteredByStudentId()
    {
        // Arrange
        var userManagerMock = MockHelpers.MockUserManager<User>();
        var companyServiceMock = new Mock<ICompanyService>();
        var studentCompanyServiceMock = new Mock<IStudentCompanyService>();
        var mapperMock = new Mock<IMapper>();

        var user = new User();
        var studentId = user.Id;
        var studentCompanies = new List<StudentCompany> { new StudentCompany
            {
                Id = 1,
                StudentId = studentId,
                Student = null,
                CompanyId = 1,
                Company = null,
                Like = false
            }
        };
        var studentCompanyDtoList = new List<StudentCompanyDto> { new StudentCompanyDto() };

        userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        userManagerMock.Setup(manager => manager.IsInRoleAsync(user, "Student")).ReturnsAsync(true);
        studentCompanyServiceMock.Setup(service => service.GetStudentCompaniesByStudentIdAsync(studentId))
            .ReturnsAsync(studentCompanies);
        mapperMock.Setup(mapper => mapper.Map<List<StudentCompany>, List<StudentCompanyDto>>(studentCompanies))
            .Returns(studentCompanyDtoList);

        var controller = new StudentCompanyController(userManagerMock.Object, companyServiceMock.Object,
            studentCompanyServiceMock.Object, mapperMock.Object);
        
        var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Student") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = await controller.GetStudentCompaniesByStudentId(studentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedStudentCompanies = Assert.IsAssignableFrom<List<StudentCompanyDto>>(okResult.Value);
        Assert.Equal(studentCompanyDtoList.Count, returnedStudentCompanies.Count);
    }
    
    [Fact]
    public async Task CreateStudentCompany_AddsLikeOrDislikeToCompany()
    {
        // Arrange
        var userManagerMock = MockHelpers.MockUserManager<User>();
        var companyServiceMock = new Mock<ICompanyService>();
        var studentCompanyServiceMock = new Mock<IStudentCompanyService>();
        var mapperMock = new Mock<IMapper>();

        var company = new Company();
        var user = new User();
        var like = true;
        var companyId = 1;

        userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        companyServiceMock.Setup(service => service.GetCompanyByIdAsync(companyId)).ReturnsAsync(company);
        studentCompanyServiceMock.Setup(service => service.CreateStudentCompanyAsync(like, companyId, user.Id))
            .ReturnsAsync(new StudentCompany());

        var controller = new StudentCompanyController(userManagerMock.Object, companyServiceMock.Object,
            studentCompanyServiceMock.Object, mapperMock.Object);

        // Act
        var result = await controller.CreateStudentCompany(like, companyId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task DeleteStudentCompany_RemovesLikeOrDislikeFromCompany()
    {
        // Arrange
        var userManagerMock = MockHelpers.MockUserManager<User>();
        var companyServiceMock = new Mock<ICompanyService>();
        var studentCompanyServiceMock = new Mock<IStudentCompanyService>();
        var mapperMock = new Mock<IMapper>();

        var user = new User();
        var studentCompany = new StudentCompany { StudentId = user.Id };
        var id = 1;

        userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        studentCompanyServiceMock.Setup(service => service.GetStudentCompanyByIdAsync(id)).ReturnsAsync(studentCompany);
        studentCompanyServiceMock.Setup(service => service.DeleteStudentCompanyAsync(id)).ReturnsAsync(true);

        var controller = new StudentCompanyController(userManagerMock.Object, companyServiceMock.Object,
            studentCompanyServiceMock.Object, mapperMock.Object);

        // Act
        var result = await controller.DeleteStudentCompany(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task GetStudentCompanies_ReturnsEmptyListWhenNoStudentCompanies()
    {
        // Arrange
        var userManagerMock = MockHelpers.MockUserManager<User>();
        var companyServiceMock = new Mock<ICompanyService>();
        var studentCompanyServiceMock = new Mock<IStudentCompanyService>();
        var mapperMock = new Mock<IMapper>();

        var user = new User();
        var studentCompanies = new List<StudentCompany>();
        var studentCompanyDtoList = new List<StudentCompanyDto>();

        userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        studentCompanyServiceMock.Setup(service => service.GetStudentCompaniesAsync()).ReturnsAsync(studentCompanies);
        mapperMock.Setup(mapper => mapper.Map<List<StudentCompany>, List<StudentCompanyDto>>(studentCompanies))
            .Returns(studentCompanyDtoList);

        var controller = new StudentCompanyController(userManagerMock.Object, companyServiceMock.Object,
            studentCompanyServiceMock.Object, mapperMock.Object);

        // Act
        var result = await controller.GetStudentCompanies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedStudentCompanies = Assert.IsAssignableFrom<List<StudentCompanyDto>>(okResult.Value);
        Assert.Empty(returnedStudentCompanies);
    }
    
    [Fact]
    public async Task CreateStudentCompany_ReturnsNotFoundWhenCompanyIdInvalid()
    {
        // Arrange
        var userManagerMock = MockHelpers.MockUserManager<User>();
        var companyServiceMock = new Mock<ICompanyService>();
        var studentCompanyServiceMock = new Mock<IStudentCompanyService>();
        var mapperMock = new Mock<IMapper>();

        var companyId = 1;

        companyServiceMock.Setup(service => service.GetCompanyByIdAsync(companyId)).ReturnsAsync((Company)null);

        var controller = new StudentCompanyController(userManagerMock.Object, companyServiceMock.Object,
            studentCompanyServiceMock.Object, mapperMock.Object);

        // Act
        var result = await controller.CreateStudentCompany(true, companyId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Company not found.", notFoundResult.Value);
    }
    
    [Fact]
    public async Task DeleteStudentCompany_ReturnsBadRequestWhenStudentTriesToDeleteLikesAndDislikesOfAnotherStudent()
    {
        // Arrange
        var userManagerMock = MockHelpers.MockUserManager<User>();
        var companyServiceMock = new Mock<ICompanyService>();
        var studentCompanyServiceMock = new Mock<IStudentCompanyService>();
        var mapperMock = new Mock<IMapper>();

        var user = new User();
        var studentCompany = new StudentCompany { StudentId = "2" };
        var id = 1;

        userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        studentCompanyServiceMock.Setup(service => service.GetStudentCompanyByIdAsync(id)).ReturnsAsync(studentCompany);

        var controller = new StudentCompanyController(userManagerMock.Object, companyServiceMock.Object,
            studentCompanyServiceMock.Object, mapperMock.Object);

        // Act
        var result = await controller.DeleteStudentCompany(id);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("A student can't delete the likes and dislikes of another student.", badRequestResult.Value);
    }
    
    [Fact]
    public async Task GetStudentCompanies_ReturnsListOfStudentCompaniesSortedByLikesAndDislikes()
    {
        // Arrange
        var userManagerMock = MockHelpers.MockUserManager<User>();
        var companyServiceMock = new Mock<ICompanyService>();
        var studentCompanyServiceMock = new Mock<IStudentCompanyService>();
        var mapperMock = new Mock<IMapper>();

        var user = new User();
        var studentCompanies = new List<StudentCompany> { new StudentCompany(), new StudentCompany() };
        var studentCompanyDtoList = new List<StudentCompanyDto> { new StudentCompanyDto(), new StudentCompanyDto() };

        userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        studentCompanyServiceMock.Setup(service => service.GetStudentCompaniesAsync()).ReturnsAsync(studentCompanies);
        mapperMock.Setup(mapper => mapper.Map<List<StudentCompany>, List<StudentCompanyDto>>(studentCompanies))
            .Returns(studentCompanyDtoList);

        var controller = new StudentCompanyController(userManagerMock.Object, companyServiceMock.Object,
            studentCompanyServiceMock.Object, mapperMock.Object);

        // Act
        var result = await controller.GetStudentCompanies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedStudentCompanies = Assert.IsAssignableFrom<List<StudentCompanyDto>>(okResult.Value);
        Assert.Equal(studentCompanyDtoList.Count, returnedStudentCompanies.Count);
    }
}