namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IUserService
{
    Task<User> CreateAsync(User user);
}
