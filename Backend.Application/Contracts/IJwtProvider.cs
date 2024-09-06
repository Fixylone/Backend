namespace Backend.Application.Contracts
{
    public interface IJwtProvider
    {
        string GenerateJwt(string userId);
    }
}