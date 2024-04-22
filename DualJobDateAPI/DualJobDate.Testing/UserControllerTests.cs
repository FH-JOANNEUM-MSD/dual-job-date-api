using System.Security.Claims;
using AutoMapper;
using DualJobDate.Api.Controllers;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DualJobDate.Testing;

public class UserControllerTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private Mock<UserManager<User>> mockUserManager = MockHelpers.MockUserManager<User>();
    private Mock<SignInManager<User>> mockSignInManager = new Mock<SignInManager<User>>(new FakeUserManager(),
        new Mock<IHttpContextAccessor>().Object,
        new Mock<IUserClaimsPrincipalFactory<User>>().Object,
        new Mock<IOptions<IdentityOptions>>().Object,
        new Mock<ILogger<SignInManager<User>>>().Object,
        new Mock<IAuthenticationSchemeProvider>().Object,
        new Mock<IUserConfirmation<User>>().Object);
    private Mock<IServiceProvider> mockServiceProvider = new Mock<IServiceProvider>();
    private Mock<IMapper> mockMapper = new Mock<IMapper>();
    private Mock<RoleManager<Role>> mockRoleManager = MockHelpers.MockRoleManager<Role>();
    private UserController controller;
    private Mock<IJwtAuthManager> mockAthManager = new Mock<IJwtAuthManager>(); 

    public UserControllerTests()
    {
        controller = new UserController(mockUserManager.Object, mockSignInManager.Object, mockServiceProvider.Object, mockMapper.Object, mockRoleManager.Object, mockAthManager.Object);
    }
    
    [Fact]
    public async Task TestLogin_GoodResult()
    {
        var loginModel = new LoginModel
        {
            Email = "admin@fh-joanneum.at",
            Password = "Administrator!1"
        };
        
        mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);
        
        //Assert
        var result = controller.Login(loginModel);
        Assert.NotNull(result.Result);
        Assert.IsType<OkObjectResult>(result.Result.Result); 
        Assert.IsNotType<UnauthorizedObjectResult>(result.Result.Result);
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
      
            mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed); 

            // Act
            var result = await controller.Login(loginModel);

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
                new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "admin@fh-joanneum.at") },
                    "mock"));

            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(existingUser);
            mockUserManager.Setup(x => x.ChangePasswordAsync(existingUser, changePasswordModel.OldPassword, newPassword))
                .ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.UpdateAsync(existingUser)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await controller.ChangePassword(changePasswordModel);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        
        [Fact]
        public async Task TestRefresh_Success()
        {
            // Arrange
            var refreshRequest = new RefreshRequest { RefreshToken = "valid_refresh_token" };

            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "user_id"));

            mockAthManager.Setup(j => j.GetPrincipalFromToken("valid_refresh_token", true)).Returns(mockPrincipal.Object);
            mockUserManager.Setup(um => um.FindByIdAsync("user_id")).ReturnsAsync(new User { Id = "user_id" });
            mockAthManager.Setup(j => j.GenerateTokens(It.IsAny<User>(), It.IsAny<DateTime>())).ReturnsAsync(new JwtAuthResultViewModel());

            // Act
            var result = await controller.Refresh(refreshRequest);

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
            mockAthManager.Setup(j => j.GetPrincipalFromToken("invalid_refresh_token", true)).Returns(new ClaimsPrincipal());

            // Act
            var result = await controller.Refresh(refreshRequest);

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
            mockPrincipal.Setup(p => p.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "non_existing_user_id"));

            mockAthManager.Setup(j => j.GetPrincipalFromToken("valid_refresh_token", true)).Returns(mockPrincipal.Object);
            mockUserManager.Setup(um => um.FindByIdAsync("non_existing_user_id")).ReturnsAsync((User)null);

            // Act
            var result = await controller.Refresh(refreshRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }


            private void SetMockClaimsUser(User user)
            {
                var httpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                    ], "mock"))
                };
                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                };
            }

        }

public class FakeSignInManager : SignInManager<User>
{
    public FakeSignInManager()
        : base(new FakeUserManager(),
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<User>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<User>>().Object)
    { }        
}

public class FakeUserManager : UserManager<User>
{
    public FakeUserManager()
        : base(new Mock<IUserStore<User>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<User>>().Object,
            new IUserValidator<User>[0],
            new IPasswordValidator<User>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<User>>>().Object)
    { }

    public override Task<IdentityResult> CreateAsync(User user, string password)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<IdentityResult> AddToRoleAsync(User user, string role)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<string> GenerateEmailConfirmationTokenAsync(User user)
    {
        return Task.FromResult(Guid.NewGuid().ToString());
    }

}