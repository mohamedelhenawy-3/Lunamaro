using Lunamaroapi.Helper.EmailSetting;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly ESetting _setting;

    public EmailService(IOptions<ESetting> setting)
    {
        _setting = setting.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_setting.SmtpHost, _setting.SmtpPort)
        {
            Credentials = new NetworkCredential(_setting.SenderEmail, _setting.SenderPassword),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_setting.SenderEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(toEmail);
        await client.SendMailAsync(mail);
    }
}

