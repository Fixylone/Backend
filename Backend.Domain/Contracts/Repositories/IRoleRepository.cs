using Backend.Domain.Entities;

namespace Backend.Domain.Contracts.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByName(string roleName);
        Task AddRole(Role role);
        Task Save();
    }
}
