using DualJobDate.BusinessLogic.Services;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Models;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace DualJobDate.Testing.Service;

public class UtilServiceTest
{
    private readonly UtilService _service;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    
    public UtilServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new UtilService(_unitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task GetInstitutionsAsync_Test()
    {
        // Arrange
        var institutions = new List<Institution>
        {
            new() { Id = 1, Name = "Institution 1" },
            new() { Id = 2, Name = "Institution 2" },
            new() { Id = 3, Name = "Institution 3" },
            new() { Id = 4, Name = "Institution 4" }
        }.AsQueryable().BuildMockDbSet();
        
        _unitOfWorkMock.Setup(service =>
                service.InstitutionRepository.GetAllAsync())
            .ReturnsAsync(institutions.Object);
        
        // Act
        var result = await _service.GetInstitutionsAsync();
        
        // Assert
        Assert.Equal(institutions.Object.ToList(), result.ToList());
    }
    
    [Theory]
    [InlineData("Institution 1", 1)]
    [InlineData("Institution 2", 2)]
    public async Task GetInstitutionByKeyNameAsync_Test(string keyName, int expectedId)
    {
        // Arrange
        var institutions = new List<Institution>
        {
            new() { Id = 1, Name = "Institution 1" },
            new() { Id = 2, Name = "Institution 2" },
            new() { Id = 3, Name = "Institution 3" },
            new() { Id = 4, Name = "Institution 4" }
        }.AsQueryable().BuildMockDbSet();
        
        _unitOfWorkMock.Setup(service =>
                service.InstitutionRepository.GetByName(keyName))
            .ReturnsAsync(institutions.Object.FirstOrDefault(i => i.Name == keyName));
        
        // Act
        var result = await _service.GetInstitutionByKeyNameAsync(keyName);
        
        // Assert
        Assert.Equal(expectedId, result.Id);
    }
    
    [Fact]
    public async Task GetAcademicProgramsAsync_Test()
    {
        // Arrange
        var academicPrograms = new List<AcademicProgram>
        {
            new() { Id = 1, Name = "Academic Program 1", InstitutionId = 1 },
            new() { Id = 2, Name = "Academic Program 2", InstitutionId = 1 },
            new() { Id = 3, Name = "Academic Program 3", InstitutionId = 2 },
            new() { Id = 4, Name = "Academic Program 4", InstitutionId = 2 }
        }.AsQueryable().BuildMockDbSet();
        
        _unitOfWorkMock.Setup(service =>
                service.AcademicProgramRepository.GetAllAsync())
            .ReturnsAsync(academicPrograms.Object);
        
        // Act
        var result = await _service.GetAcademicProgramsAsync(null);
        
        // Assert
        Assert.Equal(academicPrograms.Object.ToList(), result.ToList());
    }
    
    [Theory]
    [InlineData("Academic Program 1", 2022, 1)]
    [InlineData("Academic Program 2", 2022, 2)]
    public async Task GetAcademicProgramByKeyNameAndYearAsync_Test(string keyName, int year, int expectedId)
    {
        // Arrange
        var academicPrograms = new List<AcademicProgram>
        {
            new() { Id = 1, Name = "Academic Program 1", InstitutionId = 1, KeyName = "Academic Program 1", Year = 2022 },
            new() { Id = 2, Name = "Academic Program 2", InstitutionId = 1, KeyName = "Academic Program 2", Year = 2022 },
            new() { Id = 3, Name = "Academic Program 3", InstitutionId = 2, KeyName = "Academic Program 3", Year = 2022 },
            new() { Id = 4, Name = "Academic Program 4", InstitutionId = 2, KeyName = "Academic Program 4", Year = 2022 }
        }.AsQueryable().BuildMockDbSet();
        
        _unitOfWorkMock.Setup(service =>
                service.AcademicProgramRepository.GetByNameAndYear(keyName, year))
            .ReturnsAsync(academicPrograms.Object.FirstOrDefault(ap => ap.KeyName == keyName && ap.Year == year));
        
        // Act
        var result = await _service.GetAcademicProgramByKeyNameAndYearAsync(keyName, year);
        
        // Assert
        Assert.Equal(expectedId, result.Id);
    }
    
    [Fact]
    public async Task PostAcademicProgramAsync_Test()
    {
        // Arrange
        var academicProgram = new AcademicProgram
        {
            InstitutionId = 1,
            Year = 2022,
            Name = "Academic Program 1",
            KeyName = "Academic Program 1",
            AcademicDegreeEnum = AcademicDegreeEnum.Bachelor
        };
        
        var academicProgramModel = new AcademicProgramModel
        {
            Year = 2022,
            Name = "Academic Program 1",
            KeyName = "Academic Program 1",
            AcademicDegreeEnum = AcademicDegreeEnum.Bachelor
        };

        _unitOfWorkMock.Setup(service =>
                service.AcademicProgramRepository.GetByNameAndYear(academicProgramModel.KeyName, academicProgramModel.Year))
            .ReturnsAsync((Func<AcademicProgram>)null);
        
        // Act
        var result = await _service.PostAcademicProgramAsync(1, academicProgramModel);
        
        // Assert
        Assert.Equal(academicProgram.InstitutionId, result.InstitutionId);
        Assert.Equal(academicProgram.Year, result.Year);
        Assert.Equal(academicProgram.Name, result.Name);
        Assert.Equal(academicProgram.KeyName, result.KeyName);
        Assert.Equal(academicProgram.AcademicDegreeEnum, result.AcademicDegreeEnum);
    }
    
    [Fact]
    public async Task PostInstitutionAsync_Test()
    {
        // Arrange
        var institution = new Institution
        {
            Name = "Institution 1",
            KeyName = "Institution 1"
        };

        var institutionModel = new InstitutionModel
        {
            Name = "Institution 1",
            KeyName = "Institution 1"
        };
        
        _unitOfWorkMock.Setup(service =>
                service.InstitutionRepository.GetByName(institutionModel.KeyName))
            .ReturnsAsync((Func<Institution>)null);
        
        // Act
        var result = await _service.PostInstitutionAsync(institutionModel);
        
        // Assert
        Assert.Equal(institution.Name, result.Name);
        Assert.Equal(institution.KeyName, result.KeyName);
    }
    
    [Fact]
    public async Task PostInstitutionAsync_ThrowsException_Test()
    {
        // Arrange
        var institution = new Institution
        {
            Name = "Institution 1",
            KeyName = "Institution 1"
        };
        
        _unitOfWorkMock.Setup(service =>
                service.InstitutionRepository.GetByName("Institution 1"))
            .ReturnsAsync(institution);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.PostInstitutionAsync(new InstitutionModel
        {
            Name = "Institution 1",
            KeyName = "Institution 1"
        }));
    }
    
    [Fact]
    public async Task PutCompanyAsync_Test()
    {
        // Arrange
        var company = new Company
        {
            Name = "Company 1",
            AcademicProgramId = 1,
            InstitutionId = 1,
            UserId = "1"
        };

        var companyModel = new Company
        {
            Name = "Company 1",
            AcademicProgramId = 1,
            InstitutionId = 1,
            UserId = "1"
        };
        
        _unitOfWorkMock.Setup(service =>
                service.CompanyRepository.GetAllAsync())
            .ReturnsAsync(new List<Company>().AsQueryable().BuildMockDbSet().Object);
        
        // Act
        var result = await _service.PutCompanyAsync(companyModel.Name, companyModel.AcademicProgramId, companyModel.InstitutionId, companyModel.UserId);
        
        // Assert
        Assert.Equal(company.Name, result.Name);
        Assert.Equal(company.AcademicProgramId, result.AcademicProgramId);
        Assert.Equal(company.InstitutionId, result.InstitutionId);
        Assert.Equal(company.UserId, result.UserId);
    }
    
    [Fact]
    public async Task GetAppointmentsByUserIdAsync_Test()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = "1" },
            new() { Id = "2" },
            new() { Id = "3" },
            new() { Id = "4" }
        };
        var appointments = new List<Appointment>
        {
            new() { Id = 1, UserId = "1" , User = users[0]},
            new() { Id = 2, UserId = "2" , User = users[1]},
            new() { Id = 3, UserId = "3" , User = users[2]},
            new() { Id = 4, UserId = "4" , User = users[3]}
        }.AsQueryable().BuildMockDbSet();
        
        _unitOfWorkMock.Setup(service =>
                service.AppointmentRepository.GetAllAsync())
            .ReturnsAsync(appointments.Object);
        
        // Act
        var result = await _service.GetAppointmentsByUserIdAsync("1");
        
        // Assert
        Assert.Equal(appointments.Object.ToList().First(), result.First());
    }
    
    [Fact]
    public async Task GetAppointmentsByCompanyIdAsync_Test()
    {
        // Arrange
        var companies = new List<Company>
        {
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 },
            new() { Id = 4 }
        };
        var appointments = new List<Appointment>
        {
            new() { Id = 1, CompanyId = 1 , Company = companies[0]},
            new() { Id = 2, CompanyId = 2 , Company = companies[1]},
            new() { Id = 3, CompanyId = 3 , Company = companies[2]},
            new() { Id = 4, CompanyId = 4 , Company = companies[3]}
        }.AsQueryable().BuildMockDbSet();
        
        _unitOfWorkMock.Setup(service =>
                service.AppointmentRepository.GetAllAsync())
            .ReturnsAsync(appointments.Object);
        
        // Act
        var result = await _service.GetAppointmentsByCompanyIdAsync(1);
        
        // Assert
        Assert.Equal(appointments.Object.ToList().First(), result.First());
    }
    
}