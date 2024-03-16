namespace DualJobDate.BusinessObjects.Entities.Interface;

public interface IEmailSender
{
    void SendEmailAsync(string recipientEmail, string password);
}