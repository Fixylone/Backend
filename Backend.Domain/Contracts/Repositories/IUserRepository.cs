using Backend.Domain.Entities;

namespace Backend.Domain.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameOrEmail(string username, string email);
        Task AddUser(User user);
        Task Save();
    }
}
