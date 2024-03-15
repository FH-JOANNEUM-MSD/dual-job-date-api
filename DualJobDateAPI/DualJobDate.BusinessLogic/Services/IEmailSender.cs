namespace DualJobDate.BusinessLogic.Services;

public interface IEmailSender
{
    void SendEmailAsync(string recipientEmail, string password);
}