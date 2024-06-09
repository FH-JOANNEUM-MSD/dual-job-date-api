using DualJobDate.BusinessObjects.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DualJobDate.Testing.Fake;
public class FakeUserManager() : UserManager<User>(new Mock<IUserStore<User>>().Object,
    new Mock<IOptions<IdentityOptions>>().Object,
    new Mock<IPasswordHasher<User>>().Object, Array.Empty<IUserValidator<User>>(),
    Array.Empty<IPasswordValidator<User>>(),
    new Mock<ILookupNormalizer>().Object,
    new Mock<IdentityErrorDescriber>().Object,
    new Mock<IServiceProvider>().Object,
    new Mock<ILogger<UserManager<User>>>().Object)
{
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
