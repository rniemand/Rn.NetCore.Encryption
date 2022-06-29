using Rn.NetCore.Encryption.Configuration;

namespace Rn.NetCore.Encryption.T1.Tests.TestSupport;

public class EncryptionServiceConfigBuilder
{
  private readonly EncryptionServiceConfig _config = new();
  
  public EncryptionServiceConfigBuilder WithDefaults()
  {
    _config.Enabled = true;
    _config.Key = "rigeU7mR2zA=";
    _config.IV = "5ffasfasg4w/stkaYXm/+Mi4Aw=";
    _config.LogDecryptInput = false;
    _config.LoggingEnabled = false;
    return this;
  }

  public EncryptionServiceConfigBuilder WithDefaults(bool enabled)
    => WithDefaults().WithEnabled(enabled);

  public EncryptionServiceConfigBuilder WithKey(string key)
  {
    _config.Key = key;
    return this;
  }

  public EncryptionServiceConfigBuilder WithIV(string iv)
  {
    _config.IV = iv;
    return this;
  }

  public EncryptionServiceConfigBuilder WithLoggingEnabled(bool enabled)
  {
    _config.LoggingEnabled = enabled;
    return this;
  }

  public EncryptionServiceConfigBuilder WithLogDecryptInput(bool enabled)
  {
    _config.LogDecryptInput = enabled;
    return this;
  }

  public EncryptionServiceConfigBuilder WithEnabled(bool enabled)
  {
    _config.Enabled = enabled;
    return this;
  }

  public EncryptionServiceConfig Build() => _config;

  public EncryptionServiceConfig BuildWithDefaults()
    => WithDefaults().Build();

  public EncryptionServiceConfig BuildWithDefaults(bool enabled)
    => WithDefaults(enabled).Build();
}
