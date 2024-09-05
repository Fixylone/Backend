using Backend.Domain.Contracts.ExternalServices;
using Backend.Domain.Contracts.Repositories;
using Backend.Infrastructure.DataAccess;
using Backend.Infrastructure.DataAccess.Repositories;
using Backend.Infrastructure.ExternalServices;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure.Extensions
{
    /// <summary>
    /// Represents an configuration extension for infrastructure layer.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds services through DI for infrastructure layer.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddSingleton<IEmailService, EmailService>();
        }
    }
}
