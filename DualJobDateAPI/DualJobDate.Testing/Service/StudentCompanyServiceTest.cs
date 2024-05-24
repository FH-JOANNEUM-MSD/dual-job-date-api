using DualJobDate.BusinessLogic.Services;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace DualJobDate.Testing.Service;

public class StudentCompanyServiceTest
{
    private readonly StudentCompanyService _service;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    
    public StudentCompanyServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new StudentCompanyService(_unitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task GetStudentCompaniesAsync_Test()
    {
        // Arrange
        var studentCompanies = new List<StudentCompany>
        {
            new() { Id = 1, StudentId = "1", CompanyId = 1 },
            new() { Id = 2, StudentId = "2", CompanyId = 2 },
            new() { Id = 3, StudentId = "3", CompanyId = 3 },
            new() { Id = 4, StudentId = "4", CompanyId = 4 }
        }.AsQueryable().BuildMockDbSet();
        
        _unitOfWorkMock.Setup(service =>
                service.StudentCompanyRepository.GetAllAsync())
            .ReturnsAsync(studentCompanies.Object);
        
        // Act
        var result = await _service.GetStudentCompaniesAsync();
        
        // Assert
        Assert.Equal(studentCompanies.Object.ToList(), result);
    }
    
    [Theory]
    [InlineData("1", new[] { 1 })]
    [InlineData("2", new[] { 2 })]
    public async Task GetStudentCompaniesByStudentIdAsync_Test(string studentId, int[] expectedIds)
    {
        // Arrange
        var studentCompanies = new List<StudentCompany>
        {
            new() { Id = 1, StudentId = "1", CompanyId = 1 },
            new() { Id = 2, StudentId = "2", CompanyId = 2 },
            new() { Id = 3, StudentId = "3", CompanyId = 3 },
            new() { Id = 4, StudentId = "4", CompanyId = 4 }
        }.AsQueryable().BuildMockDbSet();
        
        _unitOfWorkMock.Setup(service =>
                service.StudentCompanyRepository.GetAllAsync())
            .ReturnsAsync(studentCompanies.Object);
        
        // Act
        var result = await _service.GetStudentCompaniesByStudentIdAsync(studentId);
        
        // Assert
        var actualIds = result.Select(c => c.Id).ToArray();
        Assert.Equal(expectedIds, actualIds);
    }
    
    [Fact]
    public async Task GetStudentCompanyByIdAsync_Test()
    {
        // Arrange
        var studentCompanies = new List<StudentCompany>
        {
            new() { Id = 1, StudentId = "1", CompanyId = 1 },
            new() { Id = 2, StudentId = "2", CompanyId = 2 },
            new() { Id = 3, StudentId = "3", CompanyId = 3 },
            new() { Id = 4, StudentId = "4", CompanyId = 4 }
        }.AsQueryable().BuildMockDbSet().Object;
        
        _unitOfWorkMock.Setup(service =>
                service.StudentCompanyRepository.GetAllAsync())
            .ReturnsAsync(studentCompanies);
        
        // Act
        var result = await _service.GetStudentCompanyByIdAsync(1);
        
        // Assert
        Assert.Equal(studentCompanies.First(), result);
    }
    
    [Fact]
    public async Task CreateStudentCompanyAsync_Test()
    {
        // Arrange
        var studentCompanies = new List<StudentCompany>
        {
            new() { Id = 1, StudentId = "1", CompanyId = 1 },
            new() { Id = 2, StudentId = "2", CompanyId = 2 },
            new() { Id = 3, StudentId = "3", CompanyId = 3 },
            new() { Id = 4, StudentId = "4", CompanyId = 4 }
        }.AsQueryable();
        
        _unitOfWorkMock.Setup(service =>
                service.StudentCompanyRepository.GetAllAsync())
            .ReturnsAsync(studentCompanies);
        
        // Act
        var result = await _service.CreateStudentCompanyAsync(true, 1, "1");
        
        // Assert
        Assert.Equal(studentCompanies.First(), result);
    }
    
    [Fact]
    public async Task DeleteStudentCompanyAsync_Test()
    {
        // Arrange
        var studentCompanies = new List<StudentCompany>
        {
            new() { Id = 1, StudentId = "1", CompanyId = 1 },
            new() { Id = 2, StudentId = "2", CompanyId = 2 },
            new() { Id = 3, StudentId = "3", CompanyId = 3 },
            new() { Id = 4, StudentId = "4", CompanyId = 4 }
        }.AsQueryable();
        
        _unitOfWorkMock.Setup(service =>
                service.StudentCompanyRepository.GetAllAsync())
            .ReturnsAsync(studentCompanies);
        
        // Act
        var result = await _service.DeleteStudentCompanyAsync(1);
        
        // Assert
        Assert.True(result);
    }
}