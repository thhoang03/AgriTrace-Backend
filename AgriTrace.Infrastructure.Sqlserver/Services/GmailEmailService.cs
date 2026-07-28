using AgriTrace.Domain.Interfaces.Outbound;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace AgriTrace.Infrastructure.Sqlserver.Services;

public class GmailEmailService : IEmailService
{
    private readonly ILogger<GmailEmailService> _logger;
    private readonly IConfiguration _configuration;

    public GmailEmailService(ILogger<GmailEmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        var host = _configuration["Smtp:Host"];
        var port = int.Parse(_configuration["Smtp:Port"] ?? "587");
        var user = _configuration["Smtp:User"];
        var pass = _configuration["Smtp:Pass"];
        var from = _configuration["Smtp:From"] ?? user;

        // Fallback to log if SMTP is not configured (useful for local dev)
        if (string.IsNullOrEmpty(host))
        {
            _logger.LogWarning(
                "[EMAIL-DEV] Host not configured. Logged instead -> To: {To} | Subject: {Subject} | Body: {Body}",
                to, subject, body);
            return;
        }

        try
        {
            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true
            };

            using var message = new MailMessage(from!, to, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("[EMAIL] Successfully sent to {To} | Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EMAIL] Failed to send email to {To}", to);
            throw;
        }
    }
}