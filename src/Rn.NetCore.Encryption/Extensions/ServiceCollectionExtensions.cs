using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rn.NetCore.Common.Exceptions;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Encryption.Configuration;

namespace Rn.NetCore.Encryption.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddRnEncryption(this IServiceCollection services, IConfiguration configuration)
  {
    // Logging
    services.TryAddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

    return services
      .AddSingleton(BindConfig(configuration))
      .AddSingleton<IEncryptionService, EncryptionService>()
      .AddSingleton<IEncryptionHelper, EncryptionHelper>();
  }

  private static EncryptionServiceConfig BindConfig(IConfiguration configuration)
  {
    const string configKey = EncryptionServiceConfig.ConfigKey;
    var boundConfig = new EncryptionServiceConfig();
    var section = configuration.GetSection(configKey);

    if (!section.Exists())
      return boundConfig;

    section.Bind(boundConfig);
    if (!boundConfig.Enabled)
      return boundConfig;

    if (string.IsNullOrWhiteSpace(boundConfig.Key))
      throw new ConfigMissingException(configKey, nameof(boundConfig.Key));

    if (string.IsNullOrWhiteSpace(boundConfig.IV))
      throw new ConfigMissingException(configKey, nameof(boundConfig.IV));

    return boundConfig;
  }
}
