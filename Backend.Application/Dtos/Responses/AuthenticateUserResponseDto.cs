namespace Backend.Application.Dtos.Responses
{
    public record AuthenticateUserResponseDto(Guid Id, string Username, string FullName, string Email,
        DateTime BirthDate, string Address, string Token);
}