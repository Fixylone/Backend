using Backend.Application.Contracts;
using Backend.Application.Dtos.Requests;
using Backend.Application.Dtos.Responses;
using Backend.Domain.Contracts.ExternalServices;
using Backend.Domain.Contracts.Repositories;
using Backend.Domain.Entities;
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
            if (request.RegisterUserRequestDto.UserType == Enums.Role.Administrator)
                throw new InvalidRoleException("You can not register as administrator.");

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
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.RegisterUserRequestDto.UserName,
                FullName = request.RegisterUserRequestDto.FullName,
                Address = request.RegisterUserRequestDto.Address,
                BirthDate = request.RegisterUserRequestDto.BirthDate,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow,
                IsActive = request.RegisterUserRequestDto.UserType == Enums.Role.User,
                RoleId = role!.Id
            };

            await _userRepository.AddUser(user);
            await _userRepository.Save();

            if (request.RegisterUserRequestDto.UserType == Enums.Role.User)
                await _emailService.SendAsync(request.RegisterUserRequestDto.Email, "Successful registration", Domain.Enums.EmailTemplate.ConfirmationEmail);
            else if (request.RegisterUserRequestDto.UserType == Enums.Role.Driver)
                await _emailService.SendAsync(request.RegisterUserRequestDto.Email, "Pending registration", Domain.Enums.EmailTemplate.PendingConfirmationEmail);

            return new GetDetailsResponseDto(user.Id, user.Username, user.FullName, user.Email,
                new RoleResponseDto(role.Id, role.Name), user.CreatedAt, user.UpdatedAt, user.LastLoginAt, user.IsActive);
        }
    }
}