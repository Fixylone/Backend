namespace Backend.Application.Dtos.Requests
{
    public record UpdateUserRequestDto(string Username, string FullName, string Email, DateTime? BirthDate, string Address, string Password);
}