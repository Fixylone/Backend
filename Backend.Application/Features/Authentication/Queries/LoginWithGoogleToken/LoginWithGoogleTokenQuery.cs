using Backend.Application.Contracts;
using Backend.Application.Dtos.Responses;
using Backend.Domain.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Authentication.Queries.LoginWithGoogleToken
{
    public record LoginWithGoogleTokenQuery(string ExternalIdentityProvider, string ExternalId, string Email)
        : IRequest<AuthenticateUserResponseDto>;

    internal sealed class LoginWithGoogleTokenQueryHandler(
        ILogger<LoginWithGoogleTokenQueryHandler> _logger,
        IUserRepository _userRepository,
        IJwtProvider _jwtProvider) : IRequestHandler<LoginWithGoogleTokenQuery, AuthenticateUserResponseDto>
    {
        /// <inheritdoc/>
        public async Task<AuthenticateUserResponseDto> Handle(LoginWithGoogleTokenQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByExternalProviderData(request.ExternalIdentityProvider, request.ExternalId);
            var now = DateTime.UtcNow;

            // Create a user if doesn't exist.
            if (user is null)
            {
                user = new Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    ExternalId = request.ExternalId,
                    ExternalIdentityProvider = request.ExternalIdentityProvider,
                    Username = $"{request.ExternalId}|{request.ExternalIdentityProvider}",
                    CreatedAt = now,
                    EmailVerificationStatus = Domain.Enums.EmailVerificationStatusEnum.Accepted
                };

                await _userRepository.AddUser(user);
            }

            user.Email = request.Email;
            user.UpdatedAt = now;
            user.LastLoginAt = now;

            await _userRepository.Save();

            return new AuthenticateUserResponseDto(user.Id, user.Username, user.FullName, user.Email,
                user.BirthDate, user.Address, _jwtProvider.GenerateJwt(user.Id.ToString()));
        }
    }
}