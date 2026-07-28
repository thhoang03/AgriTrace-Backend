using AgriTrace.Domain.Interfaces.Outbound;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AgriTrace.Infrastructure.Sqlserver.Services;

public class SendGridEmailService : IEmailService
{
    private readonly ILogger<SendGridEmailService> _logger;
    private readonly IConfiguration _configuration;

    public SendGridEmailService(ILogger<SendGridEmailService> logger, IConfiguration configuration)
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
        var apiKey = _configuration["SendGrid:ApiKey"];
        var fromEmail = _configuration["SendGrid:FromEmail"];
        var fromName = _configuration["SendGrid:FromName"] ?? "AgriTrace";

        // Fallback log nếu chưa cấu hình SendGrid ApiKey
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning(
                "[EMAIL-DEV] SendGrid ApiKey not configured. Logged instead -> To: {To} | Subject: {Subject}",
                to, subject);
            return;
        }

        try
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var toEmail = new EmailAddress(to);

            // Tự động tạo message hỗ trợ cả HTML
            var msg = MailHelper.CreateSingleEmail(
                from,
                toEmail,
                subject,
                plainTextContent: null,
                htmlContent: body
            );

            var response = await client.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("[EMAIL] SendGrid successfully sent to {To} | Subject: {Subject}", to, subject);
            }
            else
            {
                var errorBody = await response.Body.ReadAsStringAsync(cancellationToken);
                _logger.LogError("[EMAIL] SendGrid failed with StatusCode: {Status} | Response: {Body}", 
                    response.StatusCode, errorBody);
                
                throw new Exception($"SendGrid failed to send email. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EMAIL] Exception thrown while sending email via SendGrid to {To}", to);
            throw;
        }
    }
}