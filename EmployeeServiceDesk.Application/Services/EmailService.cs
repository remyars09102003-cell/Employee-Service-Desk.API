using EmployeeServiceDesk.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace EmployeeServiceDesk.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration,ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body)
    {
        try
        {
            var smtpServer =
                _configuration["Email:SmtpServer"];

            var smtpPort =
                int.Parse(
                    _configuration["Email:SmtpPort"] ?? "587");

            var username =
                _configuration["Email:Username"];

            var password =
                _configuration["Email:Password"];

            var fromEmail =
                _configuration["Email:From"];

            using var client =
                new SmtpClient(smtpServer, smtpPort);

            client.EnableSsl = true;

            client.Credentials =
                new NetworkCredential(
                    username,
                    password);

            using var mail =
                new MailMessage();

            mail.From =
                new MailAddress(fromEmail!);

            mail.To.Add(to);

            mail.Subject = subject;

            mail.Body = body;

            mail.IsBodyHtml = true;

            await client.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending email to {Email}",
                to);

            throw;
        }
    }
}