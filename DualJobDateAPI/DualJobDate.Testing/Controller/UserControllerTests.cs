using System.Security.Claims;
using AutoMapper;
using DualJobDate.Api.Controllers;
using DualJobDate.BusinessLogic.Exceptions;
using DualJobDate.BusinessObjects.Dtos;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.Testing.Fake;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DualJobDate.Testing.Controller;

public class UserControllerTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<SignInManager<User>> _mockSignInManager;
    private readonly UserController _controller;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IJwtAuthManager> _mockAuthenticationManager;
    private readonly Mock<ICompanyService> _companyServiceMock;

    public UserControllerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _mockUserManager = MockHelpers.MockUserManager<User>();
        _mockSignInManager = new Mock<SignInManager<User>>(new FakeUserManager(),
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<User>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<User>>().Object);
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockUtilService = new Mock<IUtilService>();
        var mockMapper = new Mock<IMapper>();
        var mockRoleManager = MockHelpers.MockRoleManager<Role>();
        _mockAuthenticationManager = new Mock<IJwtAuthManager>();
        _controller = new UserController(_companyServiceMock.Object,_mockUserManager.Object,
            _mockSignInManager.Object,
            mockServiceProvider.Object,
            mockMapper.Object, mockRoleManager.Object,
            _mockAuthenticationManager.Object,
            mockUtilService.Object
            );
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

        _mockUserManager.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
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

        _mockUserManager.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

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

        _mockUserManager.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _mockUserManager.Setup(manager => manager.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        // Act
        async Task Act() => await _controller.AddCompany(model);

        // Assert
        await Assert.ThrowsAsync<CompanyNotFoundException>(Act);
    }

    [Fact]
    public Task TestLogin_GoodResult()
    {
        var loginModel = new LoginModel
        {
            Email = "admin@fh-joanneum.at",
            Password = "Administrator!1"
        };

        _mockSignInManager.Setup(x =>
                x.PasswordSignInAsync(It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);

        //Assert
        var result = _controller.Login(loginModel);
        Assert.NotNull(result.Result);
        Assert.IsType<OkObjectResult>(result.Result.Result);
        Assert.IsNotType<UnauthorizedObjectResult>(result.Result.Result);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task TestLogin_InvalidEmail()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "invalidemail@domain.com",
            Password = "Administrator!1"
        };

        _mockSignInManager.Setup(x =>
                x.PasswordSignInAsync(It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _controller.Login(loginModel);

        // Assert
        Assert.NotNull(result.Result);
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task TestChangePassword_Success()
    {
        // Arrange
        var existingUser = new User { Id = "1", Email = "admin@fh-joanneum.at" };
        var newPassword = "NewPassword!2";
        var changePasswordModel = new ChangePasswordModel
        {
            OldPassword = "Administrator!1",
            NewPassword = newPassword
        };

        var claimsPrincipal =
            new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "admin@fh-joanneum.at")
                },
                "mock"));

        _mockUserManager.Setup(x =>
            x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(existingUser);
        _mockUserManager.Setup(x =>
                x.ChangePasswordAsync(existingUser, changePasswordModel.OldPassword, newPassword))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x =>
            x.UpdateAsync(existingUser)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.ChangePassword(changePasswordModel);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestRefresh_Success()
    {
        // Arrange
        var refreshRequest = new RefreshRequest { RefreshToken = "valid_refresh_token" };

        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(p => p.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(ClaimTypes.NameIdentifier, "user_id"));


        _mockAuthenticationManager.Setup(j =>
            j.GetPrincipalFromToken("valid_refresh_token", true))
            .Returns(mockPrincipal.Object);

        _mockUserManager.Setup(um =>
            um.FindByIdAsync("user_id"))
            .ReturnsAsync(new User { Id = "user_id" });

        _mockAuthenticationManager.Setup(j =>
            j.GenerateTokens(It.IsAny<User>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new JwtAuthResultViewModel());

        // Act
        var result = await _controller.Refresh(refreshRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var newTokens = Assert.IsType<JwtAuthResultViewModel>(okResult.Value);
        Assert.NotNull(newTokens);
    }

    [Fact]
    public async Task TestRefresh_InvalidToken()
    {
        // Arrange
        var refreshRequest = new RefreshRequest { RefreshToken = "invalid_refresh_token" };
        _mockAuthenticationManager.Setup(j =>
            j.GetPrincipalFromToken("invalid_refresh_token", true))
            .Returns(new ClaimsPrincipal());

        // Act
        var result = await _controller.Refresh(refreshRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid token - no user ID", unauthorizedResult.Value);
    }

    [Fact]
    public async Task TestRefresh_UserNotFound()
    {
        // Arrange
        var refreshRequest = new RefreshRequest { RefreshToken = "valid_refresh_token" };

        var mockPrincipal = new Mock<ClaimsPrincipal>();

        mockPrincipal.Setup(p =>
            p.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(ClaimTypes.NameIdentifier, "non_existing_user_id"));

        _mockAuthenticationManager.Setup(j =>
            j.GetPrincipalFromToken("valid_refresh_token", true))
            .Returns(mockPrincipal.Object);

        _mockUserManager.Setup(um =>
            um.FindByIdAsync("non_existing_user_id")).ReturnsAsync((User)null);

        // Act
        var result = await _controller.Refresh(refreshRequest);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found", notFoundResult.Value);
    }
}