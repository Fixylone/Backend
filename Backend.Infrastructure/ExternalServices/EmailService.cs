using System.Net;
using System.Reflection;
using Backend.Domain.ConfigurationModels;
using Backend.Domain.Contracts.ExternalServices;
using Backend.Domain.Enums;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Backend.Infrastructure.ExternalServices
{
    /// <summary>
    /// The email service.
    /// </summary>
    /// <seealso cref="IEmailService" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </remarks>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly AppSettingsOptions _appSettings;
        private readonly EmbeddedFileProvider _embedded;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="appSettings">The application settings.</param>
        public EmailService(ILogger<EmailService> logger, IOptions<AppSettingsOptions> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _embedded = new EmbeddedFileProvider(typeof(EmailService).GetTypeInfo().Assembly);
        }

        /// <inheritdoc />
        public async Task<bool> SendAsync(string toEmail, string subject, EmailTemplate emailTemplate)
        {
            var htmlContent = await GetHtmlContent(emailTemplate);
            var client = new SendGridClient(_appSettings.SendGridApiKey);
            var from = new EmailAddress(_appSettings.Email, _appSettings.EmailName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError($"Error sending email to {toEmail} with subject '{subject}'.\n " +
                                 $"Status code: {response.StatusCode}.\n " +
                                 $"Response body: {responseBody}");
                return false;
            }

            return true;
        }

        private async Task<string> GetHtmlContent(EmailTemplate emailTemplate)
        {
            // Prepare email template.
            await using var stream = _embedded
                .GetFileInfo($"Resources/EmailTemplates/Email_{emailTemplate}_en.html")
                .CreateReadStream();
            var emailBody = await new StreamReader(stream).ReadToEndAsync();
            emailBody = emailBody.Replace("{{APP_NAME}}", _appSettings.Name);

            return emailBody;
        }
    }
}