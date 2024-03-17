namespace DualJobDate.BusinessLogic.Services.Interface;

public interface IUserService
{
    Task<string> GetUserById(int id);
}
