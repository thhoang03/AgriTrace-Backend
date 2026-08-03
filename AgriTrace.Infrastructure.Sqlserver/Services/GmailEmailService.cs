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
        var rawPass = _configuration["Smtp:Pass"];
        var pass = rawPass?.Replace(" ", "");
        var from = _configuration["Smtp:From"] ?? user;

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            _logger.LogWarning(
                "[EMAIL-DEV] Host/User/Pass not configured. Logged instead -> To: {To} | Subject: {Subject}",
                to, subject);
            return;
        }

        try
        {
            using var client = new SmtpClient(host, port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };

            using var message = new MailMessage
            {
                From = new MailAddress(from!, "AgriTrace System"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("[EMAIL] Successfully sent email to {To} | Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EMAIL] Failed to send email to {To}. Reason: {Message}", to, ex.Message);
        }
    }
}