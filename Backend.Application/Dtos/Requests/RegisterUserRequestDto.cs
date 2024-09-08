using Backend.Domain.Enums;

namespace Backend.Application.Dtos.Requests
{
    public record RegisterUserRequestDto(string UserName, string FullName, string Email, string Password,
        string Address, DateTime BirthDate, RoleEnum UserType);

    //public byte[] UserImage { get; set; }
}