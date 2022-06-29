using NUnit.Framework;
using Rn.NetCore.Encryption.Configuration;

namespace Rn.NetCore.Encryption.T1.Tests.Configuration;

[TestFixture]
public class EncryptionServiceConfigTests
{
  [Test]
  public void EncryptionServiceConfig_GivenConstructed_ShouldDefaultEnabled() =>
    Assert.IsFalse(new EncryptionServiceConfig().Enabled);

  [Test]
  public void EncryptionServiceConfig_GivenConstructed_ShouldDefaultKey() =>
    Assert.AreEqual(string.Empty, new EncryptionServiceConfig().Key);

  [Test]
  public void EncryptionServiceConfig_GivenConstructed_ShouldDefaultIV() =>
    Assert.AreEqual(string.Empty, new EncryptionServiceConfig().IV);

  [Test]
  public void EncryptionServiceConfig_GivenConstructed_ShouldDefaultLogDecryptInput() =>
    Assert.IsFalse(new EncryptionServiceConfig().LogDecryptInput);

  [Test]
  public void EncryptionServiceConfig_GivenConstructed_ShouldDefaultLoggingEnabled() =>
    Assert.IsFalse(new EncryptionServiceConfig().LoggingEnabled);

  [Test]
  public void EncryptionServiceConfig_GivenConfigKey_ShouldReturnExpectedValue() =>
    Assert.AreEqual("Rn.Encryption", EncryptionServiceConfig.ConfigKey);
}
