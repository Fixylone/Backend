using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Application.Contracts;
using Backend.Application.Dtos.Requests;
using Backend.Application.Dtos.Responses;
using Backend.Domain.ConfigurationModels;
using Backend.Domain.Contracts.Repositories;
using Backend.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Application.Features.Authentication.Commands.AuthenticateUser
{
    public record AuthenticateUserCommand(AuthenticateUserRequestDto AuthenticateUserRequestDto) : IRequest<AuthenticateUserResponseDto>;

    internal sealed class AuthenticateUserCommandHandler(
        ILogger<AuthenticateUserCommand> _logger,
        IUserRepository _userRepository,
        IPasswordHelper _passwordHelper,
        IOptions<AppSettingsOptions> _appSettings) : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResponseDto>
    {
        /// <inheritdoc/>
        public async Task<AuthenticateUserResponseDto> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetActiveUserByEmail(request.AuthenticateUserRequestDto.Email)
                ?? throw new EmailNotExistException($"User with email '{request.AuthenticateUserRequestDto.Email}' does not exist.");

            // Check if password is correct.
            if (!_passwordHelper.VerifyHash(request.AuthenticateUserRequestDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                user.LoginFailedAt = DateTime.UtcNow;
                await _userRepository.Save();
                throw new InvalidPasswordException($"Password '{request.AuthenticateUserRequestDto.Password}' is incorrect.");
            }

            user.LoginFailedAt = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.Save();

            _logger.LogInformation("User {UserName} is successfully authenticated.", user.Username);

            return new AuthenticateUserResponseDto(user.Id, user.Username, user.FullName, user.Email, CreateToken(user.Id.ToString()));
        }

        /// <summary>
        /// Creates authentication token.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The authentication token.</returns>
        private string CreateToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Value.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, userId)]),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}