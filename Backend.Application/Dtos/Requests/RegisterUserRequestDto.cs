using Backend.Application.Enums;

namespace Backend.Application.Dtos.Requests
{
    public class RegisterUserRequestDto
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public DateTime BirthDate { get; set; }
        public Role UserType { get; set; }
        //public byte[] UserImage { get; set; }
    }
}
