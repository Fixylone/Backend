using Backend.Domain.Entities;

namespace Backend.Domain.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameOrEmail(string username, string email);

        Task<User?> GetUserByExternalProviderData(string externalIdentityProvider, string externalId);

        Task<User?> GetActiveUserByEmail(string email);

        Task<User?> GetUserById(Guid id);

        Task<User?> GetUserByEmail(string email);

        Task<bool> IsUsernameAvailable(string username);

        Task AddUser(User user);

        Task Save();
    }
}