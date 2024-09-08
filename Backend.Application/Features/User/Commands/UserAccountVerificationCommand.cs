using Backend.Application.Enums;
using Backend.Domain.Contracts.ExternalServices;
using Backend.Domain.Contracts.Repositories;
using Backend.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.User.Commands
{
    public record UserAccountVerificationCommand(string Email, EmailConfirmationEnum EmailConfirmation) : IRequest<Unit>;

    internal sealed class UserAccountVerificationCommandHandler(
        ILogger<UserAccountVerificationCommandHandler> _logger,
        IUserRepository _userRepository,
        IEmailService _emailService) : IRequestHandler<UserAccountVerificationCommand, Unit>
    {
        /// <inheritdoc/>
        public async Task<Unit> Handle(UserAccountVerificationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmail(request.Email)
                ?? throw new EntityNotFoundException($"User with email {request.Email} not found.");

            if (user.EmailVerificationStatus != Domain.Enums.EmailVerificationStatusEnum.Pending)
                throw new UserAlreadyVerifiedException($"User is already verified and it's status is {user.EmailVerificationStatus}");

            user.EmailVerificationStatus = request.EmailConfirmation == EmailConfirmationEnum.Accept ?
                Domain.Enums.EmailVerificationStatusEnum.Accepted : Domain.Enums.EmailVerificationStatusEnum.Rejected;

            await _userRepository.Save();

            await _emailService.SendAsync(request.Email, "Successful registration", Domain.Enums.EmailTemplateEnum.ConfirmationEmail);
            _logger.LogInformation("User {User} is successfully registered.", user.Username);

            return Unit.Value;
        }
    }
}