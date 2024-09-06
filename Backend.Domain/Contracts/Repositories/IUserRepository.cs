using Backend.Domain.Entities;

namespace Backend.Domain.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameOrEmail(string username, string email);

        Task<User?> GetUserByExternalProviderData(string externalIdentityProvider, string externalId);

        Task<User?> GetActiveUserByEmail(string email);

        Task AddUser(User user);

        Task Save();
    }
}