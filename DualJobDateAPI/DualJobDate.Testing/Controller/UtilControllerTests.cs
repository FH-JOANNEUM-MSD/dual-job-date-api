
using System.Security.Claims;
using AutoMapper;
using DualJobDate.Api.Controllers;
using DualJobDate.BusinessObjects.Dtos;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace DualJobDate.Testing.Controller;

public class UtilControllerTests {

    private readonly UtilController _controller;
    private readonly Mock<IUtilService> _utilServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ICompanyService> _companyServiceMock = new();
    
    public UtilControllerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<User>();
        _controller = new UtilController(_utilServiceMock.Object, _companyServiceMock.Object, _mapperMock.Object, _userManagerMock.Object);
    }
    
    [Fact]
    public async Task GetInstitutions_ReturnsListOfInstitutions()
    {
        // Arrange
        var institutions = new List<Institution>
        {
            new Institution { Id = 1, Name = "Institution 1" },
            new Institution { Id = 2, Name = "Institution 2" }
        };
        
        var institutionsMock = institutions.AsQueryable().BuildMockDbSet().Object;

        _utilServiceMock.Setup(service => service.GetInstitutionsAsync()).ReturnsAsync(institutionsMock);

        var institutionDtos = new List<InstitutionDto>
        {
            new InstitutionDto { Id = 1, Name = "Institution 1" },
            new InstitutionDto { Id = 2, Name = "Institution 2" }
        };

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<Institution>, IEnumerable<InstitutionDto>>(institutionsMock))
            .Returns(institutionDtos);

        // Act
        var result = await _controller.GetInstitutions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInstitutions = Assert.IsAssignableFrom<IEnumerable<InstitutionDto>>(okResult.Value);
        Assert.Equal(institutionDtos.Count, returnedInstitutions.Count());
    }

    [Fact]
    public async Task GetAcademicPrograms_ReturnsListOfAcademicPrograms()
    {
        // Arrange
        int? institutionId = 1;

        var academicPrograms = new List<AcademicProgram>
        {
            new AcademicProgram { Id = 1, Name = "Program 1", InstitutionId = 1 },
            new AcademicProgram { Id = 2, Name = "Program 2", InstitutionId = 1 }
        };

        var programs = academicPrograms.AsQueryable().BuildMockDbSet().Object;

        _utilServiceMock.Setup(service => service.GetAcademicProgramsAsync(institutionId)).ReturnsAsync(programs);

        var academicProgramDtos = new List<AcademicProgramDto>
        {
            new AcademicProgramDto { Id = 1, Name = "Program 1", InstitutionId = 1 },
            new AcademicProgramDto { Id = 2, Name = "Program 2", InstitutionId = 1 }
        };

        _mapperMock.Setup(mapper =>
                mapper.Map<IEnumerable<AcademicProgram>, IEnumerable<AcademicProgramDto>>(programs))
            .Returns(academicProgramDtos);

        // Act
        var result = await _controller.GetAcademicPrograms(institutionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAcademicPrograms = Assert.IsAssignableFrom<IEnumerable<AcademicProgramDto>>(okResult.Value);
        Assert.Equal(academicProgramDtos.Count, returnedAcademicPrograms.Count());
    }

    [Fact]
    public async Task PostAcademicProgram_CreatesNewAcademicProgram_ReturnsCreatedResource()
    {
        // Arrange
        int id = 1;
        var model = new AcademicProgramModel { Name = "New Program", Year = 1, KeyName = "NP", AcademicDegreeEnum = AcademicDegreeEnum.Bachelor};
        var academicProgram = new AcademicProgram { Id = id, Name = model.Name, InstitutionId = id };

        _utilServiceMock.Setup(service => service.PostAcademicProgramAsync(id, model)).ReturnsAsync(academicProgram);

        var academicProgramDto = new AcademicProgramDto { Id = id, Name = model.Name, InstitutionId = id };

        _mapperMock.Setup(mapper => mapper.Map<AcademicProgram, AcademicProgramDto>(academicProgram))
            .Returns(academicProgramDto);

        // Act
        var result = await _controller.PostAcademicProgram(id, model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAcademicProgram = Assert.IsType<AcademicProgramDto>(okResult.Value);
        Assert.Equal(academicProgramDto.Id, returnedAcademicProgram.Id);
        Assert.Equal(academicProgramDto.Name, returnedAcademicProgram.Name);
        Assert.Equal(academicProgramDto.InstitutionId, returnedAcademicProgram.InstitutionId);
    }

    [Fact]
    public async Task PostInstitution_CreatesNewInstitution_ReturnsCreatedResource()
    {
        // Arrange
        var model = new InstitutionModel { Name = "New Institution", KeyName = "NI"};
        var institution = new Institution { Id = 1, Name = model.Name };

        _utilServiceMock.Setup(service => service.PostInstitutionAsync(model)).ReturnsAsync(institution);

        var institutionDto = new InstitutionDto { Id = 1, Name = model.Name };

        _mapperMock.Setup(mapper => mapper.Map<Institution, InstitutionDto>(institution))
            .Returns(institutionDto);

        // Act
        var result = await _controller.PostInstitution(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInstitution = Assert.IsType<InstitutionDto>(okResult.Value);
        Assert.Equal(institutionDto.Id, returnedInstitution.Id);
        Assert.Equal(institutionDto.Name, returnedInstitution.Name);
    }

    [Fact]
    public async Task GetAppointmentsByUserId_ReturnsListOfAppointmentsForAuthenticatedUser()
    {
        // Arrange
        var user = new User { Id = "1" };
        var appointments = new List<Appointment>
        {
            new Appointment { Id = 1, UserId = user.Id },
            new Appointment { Id = 2, UserId = user.Id }
        };

        _userManagerMock.Setup(manager => manager.GetUserAsync(_controller.User)).ReturnsAsync(user);
        _utilServiceMock.Setup(service => service.GetAppointmentsByUserIdAsync(user.Id)).ReturnsAsync(appointments);

        var appointmentDtos = new List<AppointmentDto>
        {
            new AppointmentDto { Company = "1", UserId = user.Id },
            new AppointmentDto { Company = "2", UserId = user.Id }
        };

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDto>>(appointments))
            .Returns(appointmentDtos);

        // Act
        var result = await _controller.GetAppointmentsByUserIdAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAppointments = Assert.IsAssignableFrom<IEnumerable<AppointmentDto>>(okResult.Value);
        Assert.Equal(appointmentDtos.Count, returnedAppointments.Count());
    }

    [Fact]
    public async Task GetAppointmentsByUserId_ReturnsListOfAppointmentsForSpecifiedUser()
    {
        // Arrange
        string userId = "1";
        var appointments = new List<Appointment>
        {
            new Appointment { Id = 1, UserId = userId },
            new Appointment { Id = 2, UserId = userId }
        };

        _utilServiceMock.Setup(service => service.GetAppointmentsByUserIdAsync(userId)).ReturnsAsync(appointments);

        var appointmentDtos = new List<AppointmentDto>
        {
            new AppointmentDto { Company = "1", UserId = userId },
            new AppointmentDto { Company = "1", UserId = userId }
        };

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDto>>(appointments))
            .Returns(appointmentDtos);

        // Act
        var result = await _controller.GetAppointmentsByUserIdAsync(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAppointments = Assert.IsAssignableFrom<IEnumerable<AppointmentDto>>(okResult.Value);
        Assert.Equal(appointmentDtos.Count, returnedAppointments.Count());
    }


    [Fact]
    public async Task PostAcademicProgram_ReturnsInternalServerErrorIfUnexpectedErrorOccurs()
    {
        // Arrange
        int id = 1;
        var model = new AcademicProgramModel { Name = "New Program", Year = 1, KeyName = "NP", AcademicDegreeEnum = AcademicDegreeEnum.Bachelor};

        _utilServiceMock.Setup(service => service.PostAcademicProgramAsync(id, model)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.PostAcademicProgram(id, model);

        // Assert
        var internalServerErrorResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, internalServerErrorResult.StatusCode);
        Assert.Equal("An unexpected error occurred", internalServerErrorResult.Value);
    }

    [Fact]
    public async Task PostInstitution_ReturnsInternalServerErrorIfUnexpectedErrorOccurs()
    {
        // Arrange
        var model = new InstitutionModel { Name = "New Institution", KeyName = "NI"};

        _utilServiceMock.Setup(service => service.PostInstitutionAsync(model)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.PostInstitution(model);

        // Assert
        var internalServerErrorResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, internalServerErrorResult.StatusCode);
        Assert.Equal("An unexpected error occurred", internalServerErrorResult.Value);
    }

    [Fact]
    public async Task GetAppointmentsByCompanyIdAsync_ReturnsListOfAppointmentsForSpecifiedCompany()
    {
        // Arrange
        int companyId = 1;
        var appointments = new List<Appointment>
        {
            new Appointment { Id = 1, CompanyId = companyId },
            new Appointment { Id = 2, CompanyId = companyId }
        };
        _utilServiceMock.Setup(service => service.GetAppointmentsByCompanyIdAsync(companyId))
            .ReturnsAsync(appointments);

        var appointmentDtos = new List<AppointmentDto>
        {
            new AppointmentDto { Id = 1, CompanyId = "1" },
            new AppointmentDto { Id = 2, CompanyId = "2" }
        };
        var company = new Company()
        {
            Id = 1
        };
        var user = TestHelper.GetTestUser(1, 1, UserTypeEnum.Company);
        _userManagerMock.Setup(manager => manager.GetUserAsync(_controller.User)).ReturnsAsync(user);
        _userManagerMock.Setup(manager =>
            manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        MockClaimUser(user);

        _companyServiceMock.Setup(t => t.GetCompanyByUser(user)).ReturnsAsync(company);

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDto>>(appointments)).Returns(appointmentDtos);

        // Act
        var result = await _controller.GetAppointmentsByCompanyIdAsync(companyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAppointments = Assert.IsAssignableFrom<IEnumerable<AppointmentDto>>(okResult.Value);
        Assert.Equal(appointmentDtos.Count, returnedAppointments.Count());
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

