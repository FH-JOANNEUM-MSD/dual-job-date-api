namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IUserService
{
    Task<string> GetUserById(int id);
}
