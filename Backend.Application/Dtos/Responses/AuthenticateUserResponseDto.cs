namespace Backend.Application.Dtos.Responses
{
    public record AuthenticateUserResponseDto(Guid Id, string Username, string FullName, string Email, string Token);
}