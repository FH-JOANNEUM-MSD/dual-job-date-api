using DualJobDate.BusinessObjects.Entities;
using Microsoft.AspNetCore.Identity;

namespace DualJobDate.Testing.Fake;

public class FakeUserStore : IUserStore<User>
{
    public void Dispose()
    {
    }
    
    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }
    
    public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }
    
    public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }
    
    public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }
    
    public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }
    
    public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }
    
    public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }
    
    public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }
    
    public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return Task.FromResult(new User { Id = userId });
    }
    
    public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return Task.FromResult(new User { NormalizedUserName = normalizedUserName });
    }
}