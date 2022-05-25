using Microsoft.Extensions.Configuration;
using Rn.NetCore.Common.Exceptions;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Encryption.Configuration;

namespace Rn.NetCore.Encryption.Providers;

public interface IEncryptionConfigProvider
{
  EncryptionServiceConfig Provide();
}

public class EncryptionConfigProvider : IEncryptionConfigProvider
{
  private readonly ILoggerAdapter<EncryptionConfigProvider> _logger;
  private readonly IConfiguration _configuration;

  public EncryptionConfigProvider(
    ILoggerAdapter<EncryptionConfigProvider> logger,
    IConfiguration configuration)
  {
    _logger = logger;
    _configuration = configuration;
  }
  
  public EncryptionServiceConfig Provide()
  {
    const string configKey = EncryptionServiceConfig.ConfigKey;
    var boundConfig = new EncryptionServiceConfig();
    var section = _configuration.GetSection(configKey);

    // Handle configuration missing
    if (!section.Exists())
    {
      _logger.LogWarning(
        "Unable to find configuration section '{section}', " +
        "as a result the EncryptionService will be disabled.",
        configKey);

      return boundConfig;
    }

    // Bind and process the configuration
    section.Bind(boundConfig);
    if (!boundConfig.Enabled)
      return boundConfig;

    // Ensure that the "Key" and "IV" has been set
    if (string.IsNullOrWhiteSpace(boundConfig.Key))
      throw new ConfigMissingException(configKey, nameof(boundConfig.Key));

    if (string.IsNullOrWhiteSpace(boundConfig.IV))
      throw new ConfigMissingException(configKey, nameof(boundConfig.IV));

    return boundConfig;
  }
}
