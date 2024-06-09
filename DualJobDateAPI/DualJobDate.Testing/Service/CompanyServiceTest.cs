using DualJobDate.BusinessLogic.Services;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using DualJobDate.BusinessObjects.Entities.Models;
using Moq;
using Xunit;

namespace DualJobDate.Testing.Service;

public class CompanyServiceTest
{
    private readonly CompanyService _service;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CompanyServiceTest()
    {
        var userManagerMock = MockHelpers.MockUserManager<User>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new CompanyService(_unitOfWorkMock.Object, userManagerMock.Object);
    }

    [Theory]
    [InlineData(1, new[] { 1, 3 })]
    [InlineData(2, new[] { 2, 4 })]
    public async Task GetCompaniesByAcademicProgramAsync_Test(int academicProgramId, int[] expectedIds)
    {
        // Arrange
        var companies = new List<Company>
        {
            new() { Id = 1, InstitutionId = 1, AcademicProgramId = 1 },
            new() { Id = 2, InstitutionId = 1, AcademicProgramId = 2 },
            new() { Id = 3, InstitutionId = 2, AcademicProgramId = 1 },
            new() { Id = 4, InstitutionId = 2, AcademicProgramId = 2 }
        }.AsQueryable();

        _unitOfWorkMock.Setup(service =>
                service.CompanyRepository.GetAllAsync())
            .ReturnsAsync(companies);

        // Act
        var result = await _service.GetCompaniesByAcademicProgramAsync(academicProgramId);

        // Assert
        var actualIds = result.Select(c => c.Id).ToArray();
        Assert.Equal(expectedIds, actualIds);
    }

    [Theory]
    [InlineData(1, new[] { 1, 2 })]
    [InlineData(2, new[] { 3, 4 })]
    public async Task GetCompaniesByInstitutionAsync_Test(int institutionId, int[] expectedIds)
    {
        // Arrange
        var companies = new List<Company>
        {
            new() { Id = 1, InstitutionId = 1, AcademicProgramId = 1 },
            new() { Id = 2, InstitutionId = 1, AcademicProgramId = 2 },
            new() { Id = 3, InstitutionId = 2, AcademicProgramId = 1 },
            new() { Id = 4, InstitutionId = 2, AcademicProgramId = 2 }
        }.AsQueryable();

        _unitOfWorkMock.Setup(service =>
                service.CompanyRepository.GetAllAsync())
            .ReturnsAsync(companies);

        // Act
        var result = await _service.GetCompaniesByInstitutionAsync(institutionId);

        // Assert
        var actualIds = result.Select(c => c.Id).ToArray();
        Assert.Equal(expectedIds, actualIds);
    }

    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 2)]
    public async Task UpdateCompanyActivity_UpdatesActivityAndCommitsTransaction(bool newActivityState, int companyId)
    {
        // Arrange
        var company = new Company { Id = companyId, IsActive = !newActivityState };


        var mockCompanyRepository = new Mock<ICompanyRepository>();
        _unitOfWorkMock.Setup(u => u.CompanyRepository).Returns(mockCompanyRepository.Object);

        // Act
        await _service.UpdateCompanyActivity(newActivityState, company);

        // Assert
        mockCompanyRepository.Verify(x => x.UpdateAsync(company), Times.Once);
        _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);

        Assert.Equal(newActivityState, company.IsActive);
    }

    [Fact]
    public async Task UpdateCompany_ValidModel_UpdatesCompanyAndCommitsTransaction()
    {
        // Arrange
        var model = new UpdateCompanyModel
        {
            Name = "Test Company",
            Industry = "Technology",
            Website = "https://example.com",
            LogoBase64 = "base64string"
        };

        var company = new Company { Id = 1 };

        var mockCompanyRepository = new Mock<ICompanyRepository>();
        _unitOfWorkMock.Setup(u => u.CompanyRepository).Returns(mockCompanyRepository.Object);

        // Act
        await _service.UpdateCompany(model, company);

        // Assert
        mockCompanyRepository.Verify(x => x.UpdateAsync(company), Times.Once);
        _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
    }

    [Theory]
    [InlineData(null, "Technology", "https://example.com", "base64string")]
    [InlineData("Test Company", null, "https://example.com", "base64string")]
    [InlineData("Test Company", "Technology", "invalidurl", "base64string")]
    [InlineData("Test Company", "Technology", "https://example.com", "invalidbase64")]
    public async Task UpdateCompany_InvalidModel_ThrowsArgumentException(string name, string industry, string website,
        string logoBase64)
    {
        // Arrange
        var model = new UpdateCompanyModel
        {
            Name = name,
            Industry = industry,
            Website = website,
            LogoBase64 = logoBase64
        };

        var company = new Company { Id = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateCompany(model, company));
    }
}