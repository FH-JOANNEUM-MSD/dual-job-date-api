using DualJobDate.BusinessLogic.Services;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DualJobDate.Testing.Service;

public class CompanyServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CompanyService _service;

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
            new() { Id = 4, InstitutionId = 2, AcademicProgramId = 2 },
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
            new() { Id = 4, InstitutionId = 2, AcademicProgramId = 2 },
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
}
