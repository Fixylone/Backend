namespace Backend.Application.Dtos.Requests
{
    public record AuthenticateUserRequestDto(string Email, string Password);
}