using Backend.Domain.Enums;

namespace Backend.Domain.Contracts.ExternalServices
{
    /// <summary>
    /// The email service interface.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the email asynchronously.
        /// </summary>
        /// <param name="toEmail">The recipient email address.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="emailTemplate">The email template.</param>
        /// <returns><c>true</c> if sending was successful, <c>false</c> otherwise.</returns>
        Task<bool> SendAsync(string toEmail, string subject, EmailTemplate emailTemplate);
    }
}
