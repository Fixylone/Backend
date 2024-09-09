using Backend.Application.Contracts;
using Backend.Application.Middlewares;
using Backend.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Application.Extensions
{
    /// <summary>
    /// Represents the extension method for configuring application layer services.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds application layer services.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddSingleton<IPasswordHelper, PasswordHelper>();
            services.AddSingleton<IJwtProvider, JwtProvider>();

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(AssemblyReference.Assembly);
            });
        }

        /// <summary>
        /// Adds middlewares from application layer.
        /// </summary>
        /// <param name="app">Application builder.</param>
        public static void UseApplicationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}