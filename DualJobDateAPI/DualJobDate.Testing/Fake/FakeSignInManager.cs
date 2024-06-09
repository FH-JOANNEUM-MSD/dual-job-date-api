using DualJobDate.BusinessObjects.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DualJobDate.Testing.Fake;
public class FakeSignInManager() : SignInManager<User>(new FakeUserManager(),
    new Mock<IHttpContextAccessor>().Object,
    new Mock<IUserClaimsPrincipalFactory<User>>().Object,
    new Mock<IOptions<IdentityOptions>>().Object,
    new Mock<ILogger<SignInManager<User>>>().Object,
    new Mock<IAuthenticationSchemeProvider>().Object,
    new Mock<IUserConfirmation<User>>().Object);
