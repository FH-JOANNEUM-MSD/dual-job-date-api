namespace DualJobDate.BusinessObjects.Entities.Interface.Helper
{
    public interface IEmailHelper
    {
        void SendEmailAsync(string recipientEmail, string password);
    }
}