using Backend.Domain.Contracts.Repositories;
using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.DataAccess.Repositories
{
    public class UserRepository(ApplicationDbContext _applicationDbContext) : IUserRepository
    {
        public async Task AddUser(User user)
        {
            await _applicationDbContext.Users.AddAsync(user);
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _applicationDbContext.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetActiveUserByEmail(string email)
        {
            return await _applicationDbContext.Users
                .FirstOrDefaultAsync(x => x.Email == email && x.EmailVerificationStatus == Domain.Enums.EmailVerificationStatusEnum.Accepted);
        }

        public async Task<User?> GetUserByExternalProviderData(string externalIdentityProvider, string externalId)
        {
            return await _applicationDbContext.Users
                .SingleOrDefaultAsync(x => x.ExternalIdentityProvider == externalIdentityProvider && x.ExternalId == externalId);
        }

        public async Task<User?> GetUserByUsernameOrEmail(string username, string email)
        {
            return await _applicationDbContext.Users
                .FirstOrDefaultAsync(x => x.Username == username || x.Email == email);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _applicationDbContext.Users
               .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> IsUsernameAvailable(string username)
        {
            return !(await _applicationDbContext.Users.AnyAsync(x => x.Username == username));
        }

        public async Task Save()
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}