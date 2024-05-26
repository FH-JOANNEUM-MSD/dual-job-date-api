using System.Security.Claims;
using AutoMapper;
using DualJobDate.Api.Controllers;
using DualJobDate.BusinessObjects.Dtos;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
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
using MockQueryable.Moq;
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
    private readonly Mock<IJwtAuthManager> _mockAuthenticationManager;
    private readonly Mock<IUtilService> _mockUtilService;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<RoleManager<Role>> _mockRoleManager;
    private readonly Mock<IMapper> _mockMapper;

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
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockUtilService = new Mock<IUtilService>();
        _mockMapper = new Mock<IMapper>();
        _mockRoleManager = MockHelpers.MockRoleManager<Role>();
        _mockAuthenticationManager = new Mock<IJwtAuthManager>();
        _controller = new UserController(_mockUserManager.Object,
            _mockSignInManager.Object,
            _mockServiceProvider.Object,
            _mockMapper.Object, _mockRoleManager.Object,
            _mockAuthenticationManager.Object,
            _mockUtilService.Object
        );
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
            .ReturnsAsync(new JwtAuthResultViewDto());

        // Act
        var result = await _controller.Refresh(refreshRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var newTokens = Assert.IsType<JwtAuthResultViewDto>(okResult.Value);
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
    
    [Fact]
    public async Task RegisterUser_Success()
    {
        // Arrange
        var registerUserModel = new RegisterUserModel { Email = "test@test.com", Role = UserTypeEnum.Student, InstitutionId = 1, AcademicProgramId = 1 };
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _mockUtilService.Setup(x => x.GetInstitutionsAsync())
            .ReturnsAsync(new List<Institution> { new Institution { Id = 1, Name = "Test Institution" } }.AsQueryable());
        _mockUtilService.Setup(x => x.GetAcademicProgramsAsync(null))
            .ReturnsAsync(new List<AcademicProgram> { new AcademicProgram { Id = 1, Name = "Test Program" } }.AsQueryable());

        _mockServiceProvider.Setup(x => x.GetService(typeof(IUserStore<User>)))
            .Returns(new FakeUserStore());
        _mockRoleManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new Role { Name = "Student" });
        
        var mockData = new List<Role>
        {
            new Role { UserTypeEnum = UserTypeEnum.Admin },
            new Role { UserTypeEnum = UserTypeEnum.Company },
            new Role { UserTypeEnum = UserTypeEnum.Student, Name = "Student"}
        };

        var mockSet = mockData.AsQueryable().BuildMockDbSet();

        _mockRoleManager.Setup(rm => rm.Roles).Returns(mockSet.Object);

        var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        // Act
        var result = await _controller.RegisterUser(registerUserModel);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Contains("created successfully", ((OkObjectResult)result).Value.ToString());
    }
    
    [Fact]
    public async Task GetName_Test()
    {
        // Arrange
        var user = new User { FirstName = "Test", LastName = "User", Email = "t.t@test.com"};
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetName();

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var okResult = (OkObjectResult)result.Result;
        NameAndEmailDto value = (NameAndEmailDto)okResult.Value;
        Assert.Equal(user.FirstName, value.FirstName);
        Assert.Equal(user.LastName, value.LastName);
        Assert.Equal(user.Email, value.Email);
    }
    
    [Fact]
    public async Task GetName_Test_UserNotFound()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _controller.GetName();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task ChangeName_Test()
    {
        // Arrange
        var user = new User { FirstName = "Test", LastName = "User", Email = "t.t@test.com" };
        var nameModel = new ChangeNameModel { FirstName = "New", LastName = "Name" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _controller.ChangeName(nameModel);
        
        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;
        
        Assert.Contains("Name changed successfully", okResult.Value.ToString());
    }

    [Fact]
    public async Task DeleteUserWithPassword_Test()
    {
        // Arrange
        var user = new User { FirstName = "Test", LastName = "User", Email = "t.t@test.com" };
        var passwordModel = new DeleteUserModel
        {
            Password = "TestPassword!1",
            Email = user.Email
        };

        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, passwordModel.Password, false))
            .ReturnsAsync(SignInResult.Success);

        _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.DeleteUserWithPassword(passwordModel);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;

        Assert.Contains("User deleted successfully", okResult.Value.ToString());
    }

    [Fact]
    public async Task DeleteUser_Test()
    {
        // Arrange
        var user = new User { FirstName = "Test", LastName = "User", Email = "t.t@test.com" };
        _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.DeleteUser(user.Id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;

        Assert.Contains("User deleted successfully", okResult.Value.ToString());
    }
    
    [Fact]
    public async Task DeleteUser_Test_UserNotFound()
    {
        // Arrange
        _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _controller.DeleteUser("1");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetUser_Test()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = "1", FirstName = "Test", LastName = "User", Email = "t.t@test.com", IsNew = false, UserType = UserTypeEnum.Admin, InstitutionId = 1, AcademicProgramId = 1 }
        }.AsQueryable().BuildMockDbSet().Object;
        _mockUserManager.Setup(x => x.Users)
            .Returns(users);
        _mockMapper.Setup(x => x.Map<User, UserDto>(It.IsAny<User>()))
            .Returns(new UserDto { Id = "1", Institution = new InstitutionDto { Id = 1, Name = "Test Institution", KeyName = "Test "}, AcademicProgram = new AcademicProgramDto { Id = 1, Name = "Test Program" }, UserType = UserTypeEnum.Admin, IsNew = false, Email = "t.t@test.com" });

        // Act
        var result = await _controller.GetUser(users.First().Id);

        // Assert
        var resultResult = (OkObjectResult)result.Result;
        Assert.IsType<UserDto>(resultResult.Value);
        Assert.Equal(users.First().Id, ((UserDto)resultResult.Value).Id);
    }

}