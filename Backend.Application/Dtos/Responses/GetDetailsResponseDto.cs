using Backend.Domain.Enums;

namespace Backend.Application.Dtos.Responses
{
    public record GetDetailsResponseDto(
        Guid Id,
        string Username,
        string FullName,
        string Email,
        RoleResponseDto RoleData,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        DateTime? LastLoginAt,
        EmailVerificationStatusEnum EmailVerificationStatus);
}