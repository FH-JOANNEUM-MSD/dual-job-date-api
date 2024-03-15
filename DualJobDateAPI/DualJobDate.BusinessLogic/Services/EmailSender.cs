using System.Net;
using System.Net.Mail;
using System.Reflection;

namespace DualJobDate.BusinessLogic.Services;

public class EmailSender : IEmailSender
{
    public void SendEmailAsync(string recipientEmail, string recipientPassword)
    {
        Task.Run(async () =>
        {
            var senderEmail = "emanuelneziraj@gmail.com";
            var senderPassword = "eqlv lzgq bati juii";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
            };
            var invitationLink = $"http://localhost:4200/Login";
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = "Registrierung DualJobDate",
                Body = await GetEmailTemplate(invitationLink, recipientEmail, recipientPassword),
                IsBodyHtml = true,
            };
            mailMessage.To.Add(recipientEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Einladung an {recipientEmail} gesendet.");
            }
            catch (Exception ex)
            {
                // Hier sollten Sie den Fehler protokollieren
                Console.WriteLine($"Fehler beim Senden der E-Mail: {ex.Message}");
            }
        });
    }
    
    private static async Task<string> GetEmailTemplate(string invitationLink, string email, string password)
    {
        var assembly = Assembly.GetExecutingAssembly();
        const string templatePath = "DualJobDate.BusinessLogic.Templates.email.html";
        await using var stream = assembly.GetManifestResourceStream(templatePath);
        
        using var reader = new StreamReader(stream);

        var content = await reader.ReadToEndAsync();
        content = content.Replace("{{email}}", email)
            .Replace("{{InvitationLink}}", invitationLink)
            .Replace("{{temporaryPassword}}", password);
        return content;
    }


}