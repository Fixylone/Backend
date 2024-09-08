using Backend.Application.Contracts;
using Backend.Application.Dtos.Requests;
using Backend.Application.Dtos.Responses;
using Backend.Domain.Contracts.ExternalServices;
using Backend.Domain.Contracts.Repositories;
using Backend.Domain.Enums;
using Backend.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Authentication.Commands.RegisterUser
{
    public record RegisterUserCommand(RegisterUserRequestDto RegisterUserRequestDto) : IRequest<GetDetailsResponseDto>;

    internal sealed class RegisterUserCommandHandler(
        ILogger<RegisterUserCommandHandler> _logger,
        IUserRepository _userRepository,
        IRoleRepository _roleRepository,
        IPasswordHelper _passwordHelper,
        IEmailService _emailService) : IRequestHandler<RegisterUserCommand, GetDetailsResponseDto>
    {
        /// <inheritdoc/>
        public async Task<GetDetailsResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RegisterUserRequestDto.Password))
                throw new InvalidPasswordException("Password is required.");

            var existingUser = await _userRepository.GetUserByUsernameOrEmail
                (request.RegisterUserRequestDto.UserName, request.RegisterUserRequestDto.Email);

            if (existingUser?.Username == request.RegisterUserRequestDto.UserName)
                throw new UsernameTakenException($"Username '{request.RegisterUserRequestDto.UserName}' is already taken.");

            if (existingUser?.Email == request.RegisterUserRequestDto.Email)
                throw new EmailTakenException($"Email '{request.RegisterUserRequestDto.Email}' is already taken.");

            var (passwordHash, passwordSalt) = _passwordHelper.CreateHash(request.RegisterUserRequestDto.Password);

            var role = await _roleRepository.GetRoleByName(request.RegisterUserRequestDto.UserType.ToString());
            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Username = request.RegisterUserRequestDto.UserName,
                FullName = request.RegisterUserRequestDto.FullName,
                Address = request.RegisterUserRequestDto.Address,
                BirthDate = request.RegisterUserRequestDto.BirthDate,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = request.RegisterUserRequestDto.Email,
                CreatedAt = DateTime.UtcNow,
                EmailVerificationStatus = request.RegisterUserRequestDto.UserType == RoleEnum.User
                ? EmailVerificationStatusEnum.Accepted : EmailVerificationStatusEnum.Pending,
                RoleId = role!.Id
            };

            await _userRepository.AddUser(user);
            await _userRepository.Save();

            if (request.RegisterUserRequestDto.UserType == RoleEnum.Driver)
                await _emailService.SendAsync(request.RegisterUserRequestDto.Email, "Pending registration", Domain.Enums.EmailTemplateEnum.PendingConfirmationEmail);
            else
            {
                await _emailService.SendAsync(request.RegisterUserRequestDto.Email, "Successful registration", Domain.Enums.EmailTemplateEnum.ConfirmationEmail);
                _logger.LogInformation("User {User} is successfully registered.", request.RegisterUserRequestDto.UserName);
            }

            return new GetDetailsResponseDto(user.Id, user.Username, user.FullName, user.Email,
                new RoleResponseDto(role.Id, role.Name), user.CreatedAt, user.UpdatedAt, user.LastLoginAt, user.EmailVerificationStatus);
        }
    }
}