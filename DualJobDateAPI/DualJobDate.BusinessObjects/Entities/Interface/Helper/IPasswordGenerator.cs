namespace DualJobDate.BusinessObjects.Entities.Interface.Helper;

public interface IPasswordGenerator
{
    string GeneratePassword(int length = 12);
}
