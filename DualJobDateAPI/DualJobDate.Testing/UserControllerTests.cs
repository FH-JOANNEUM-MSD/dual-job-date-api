using AutoMapper;
using DualJobDate.Api.Controllers;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DualJobDate.Testing;

public class UserControllerTests
{
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

    public UserControllerTests()
    {
        // setup for tests
        controller = new UserController(mockUserManager.Object, mockSignInManager.Object, mockServiceProvider.Object, mockMapper.Object, mockRoleManager.Object);
    }
    
    //test Login
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
        var result = controller.Login(loginModel, false, false);
        Assert.NotNull(result);
        Assert.IsType<EmptyHttpResult>(result.Result.Result);   //empty bc signinManager is handing the result to the user
        Assert.IsNotType<ProblemHttpResult>(result.Result.Result);
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