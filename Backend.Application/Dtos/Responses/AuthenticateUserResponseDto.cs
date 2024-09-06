namespace Backend.Application.Dtos.Responses
{
    public record AuthenticateUserResponseDto(Guid Id, string Username, string FullName, string Email,
        DateTime DateOfBirth, string Address, string Token);
}