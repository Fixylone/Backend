using Backend.Domain.ConfigurationModels;
using Microsoft.Extensions.Options;

namespace Backend.OptionsSetup
{
    public class AppSettingsOptionsSetup(IConfiguration _configuration) : IConfigureOptions<AppSettingsOptions>
    {
        /// <inheritdoc/>
        public void Configure(AppSettingsOptions options)
        {
            _configuration.GetSection(AppSettingsOptions.AppSettingsConfigurationSectionName).Bind(options);
        }
    }
}
