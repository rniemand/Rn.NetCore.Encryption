using Microsoft.Extensions.Configuration;

namespace Rn.NetCore.Encryption.Configuration;

// DOCS: docs\configuration\EncryptionServiceConfig.md
public class EncryptionServiceConfig
{
  public const string ConfigKey = "Rn.Encryption";

  [ConfigurationKeyName("enabled")]
  public bool Enabled { get; set; }

  [ConfigurationKeyName("key")]
  public string Key { get; set; } = string.Empty;

  [ConfigurationKeyName("iv")]
  public string IV { get; set; } = string.Empty;

  [ConfigurationKeyName("loggingEnabled")]
  public bool LoggingEnabled { get; set; }

  [ConfigurationKeyName("logDecryptInput")]
  public bool LogDecryptInput { get; set; }
}
