using Backend.Domain.Contracts.Repositories;
using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.DataAccess.Repositories
{
    public sealed class RoleRepository(ApplicationDbContext _applicationDbContext) : IRoleRepository
    {
        public async Task AddRole(Role role)
        {
            await _applicationDbContext.Roles.AddAsync(role);
        }

        public async Task<Role?> GetRoleByName(string roleName)
        {
            return await _applicationDbContext.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
        }

        public async Task Save()
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
