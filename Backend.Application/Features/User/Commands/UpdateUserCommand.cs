using System.Security.Claims;
using Backend.Application.Contracts;
using Backend.Application.Dtos.Requests;
using Backend.Application.Dtos.Responses;
using Backend.Domain.Contracts.Repositories;
using Backend.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.User.Commands
{
    public record UpdateUserCommand(UpdateUserRequestDto UpdateUserRequestDto)
        : IRequest<GetDetailsResponseDto>;

    internal sealed class UpdateUserCommandHandler(
        ILogger<UpdateUserCommand> _logger,
        IPasswordHelper _passwordHelper,
        IUserRepository _userRepository,
        IHttpContextAccessor _httpContextAccessor) : IRequestHandler<UpdateUserCommand, GetDetailsResponseDto>
    {
        /// <inheritdoc/>
        public async Task<GetDetailsResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var claimsIdentity = _httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity
                ?? throw new UnauthorizedAccessException();

            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.Name).Value);
            var user = await _userRepository.GetUserById(userId)
                ?? throw new EntityNotFoundException("User not found.");

            var isExternalUser = !string.IsNullOrEmpty(user.ExternalId);
            if (isExternalUser)
                throw new UpdateReadOnlyPropertyException("Cannot update externally signed in user data.");

            // Update username if it has changed.
            if (request.UpdateUserRequestDto.Username is not null && request.UpdateUserRequestDto.Username != user.Username)
            {
                // Throw error if the new username is already taken.
                if (await _userRepository.IsUsernameAvailable(request.UpdateUserRequestDto.Username))
                    throw new UsernameTakenException($"Username '{request.UpdateUserRequestDto.Username}' is already taken.");
                user.Username = request.UpdateUserRequestDto.Username;
            }

            user.FullName = request.UpdateUserRequestDto.FullName is not null ? request.UpdateUserRequestDto.FullName : user.FullName;
            user.BirthDate = request.UpdateUserRequestDto.BirthDate is not null ? (DateTime)request.UpdateUserRequestDto.BirthDate : user.BirthDate;
            user.Address = request.UpdateUserRequestDto.Address is not null ? request.UpdateUserRequestDto.Address : user.Address;
            user.Email = request.UpdateUserRequestDto.Email is not null ? request.UpdateUserRequestDto.Email : user.Email;

            // Update password if provided.
            if (request.UpdateUserRequestDto.Password is not null)
            {
                var (passwordHash, passwordSalt) = _passwordHelper.CreateHash(request.UpdateUserRequestDto.Password);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedById = userId;

            await _userRepository.Save();

            return new GetDetailsResponseDto(user.Id, user.Username, user.FullName, user.Email,
                new RoleResponseDto((Guid)user.RoleId, user.Role.Name), user.CreatedAt, user.UpdatedAt, user.LastLoginAt, user.EmailVerificationStatus);
        }
    }
}