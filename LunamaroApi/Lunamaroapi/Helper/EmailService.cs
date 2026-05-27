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

    // ✅ Core sender — unchanged, used by background service
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

    public string BuildWelcomeHtml(string toEmail, string userName) => $@"
        <div style='font-family:sans-serif;max-width:600px;margin:auto;background:#0f1c2e;color:#fff;border-radius:12px;overflow:hidden;'>
          <div style='background:#efb036;padding:30px;text-align:center;'>
            <h1 style='margin:0;color:#0f1c2e;font-size:2rem;'>🌙 Lunamaro</h1>
          </div>
          <div style='padding:40px;'>
            <h2 style='color:#efb036;'>Welcome, {userName}! 🎉</h2>
            <p style='color:rgba(255,255,255,0.8);line-height:1.8;'>
              Your account has been successfully created using Google.<br>
              We're excited to have you at Lunamaro Restaurant.
            </p>
            <div style='margin:30px 0;padding:20px;background:rgba(255,255,255,0.05);border-radius:8px;border-left:4px solid #efb036;'>
              <p style='margin:0;color:rgba(255,255,255,0.6);font-size:0.9rem;'>Account Email</p>
              <p style='margin:5px 0 0;color:#fff;font-weight:bold;'>{toEmail}</p>
            </div>
            <a href='https://lunamaro.netlify.app'
               style='display:inline-block;background:#efb036;color:#0f1c2e;padding:14px 35px;border-radius:8px;text-decoration:none;font-weight:bold;margin-top:10px;'>
              Explore Our Menu
            </a>
          </div>
          <div style='padding:20px;text-align:center;background:rgba(255,255,255,0.03);'>
            <p style='color:rgba(255,255,255,0.3);font-size:0.8rem;margin:0;'>© 2026 Lunamaro Restaurant — Giza, Egypt</p>
          </div>
        </div>";

    public string BuildLoginNotificationHtml(string userName)
    {
        var time = DateTime.UtcNow.ToString("dddd, MMMM d yyyy 'at' HH:mm 'UTC'");
        return $@"
        <div style='font-family:sans-serif;max-width:600px;margin:auto;background:#0f1c2e;color:#fff;border-radius:12px;overflow:hidden;'>
          <div style='background:#efb036;padding:30px;text-align:center;'>
            <h1 style='margin:0;color:#0f1c2e;font-size:2rem;'>🌙 Lunamaro</h1>
          </div>
          <div style='padding:40px;'>
            <h2 style='color:#efb036;'>New Login Detected 🔐</h2>
            <p style='color:rgba(255,255,255,0.8);line-height:1.8;'>
              Hi {userName}, we noticed a new login to your Lunamaro account.
            </p>
            <div style='margin:30px 0;padding:20px;background:rgba(255,255,255,0.05);border-radius:8px;border-left:4px solid #efb036;'>
              <p style='margin:0 0 8px;color:rgba(255,255,255,0.5);font-size:0.85rem;'>LOGIN TIME</p>
              <p style='margin:0;color:#efb036;font-weight:bold;'>{time}</p>
            </div>
            <p style='color:rgba(255,255,255,0.5);font-size:0.9rem;'>
              If this was you, no action is needed.<br>
              If you didn't log in, please contact us immediately.
            </p>
          </div>
          <div style='padding:20px;text-align:center;background:rgba(255,255,255,0.03);'>
            <p style='color:rgba(255,255,255,0.3);font-size:0.8rem;margin:0;'>© 2026 Lunamaro Restaurant — Giza, Egypt</p>
          </div>
        </div>";
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        => await SendEmailAsync(toEmail, "Welcome to Lunamaro! 🌙", BuildWelcomeHtml(toEmail, userName));

    public async Task SendLoginNotificationAsync(string toEmail, string userName)
        => await SendEmailAsync(toEmail, "New login to your Lunamaro account 🔐", BuildLoginNotificationHtml(userName));
}