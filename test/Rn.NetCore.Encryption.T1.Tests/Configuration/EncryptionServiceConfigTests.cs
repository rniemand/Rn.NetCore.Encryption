using NUnit.Framework;
using Rn.NetCore.Encryption.Configuration;

namespace Rn.NetCore.Encryption.T1.Tests.Configuration;

[TestFixture]
public class EncryptionServiceConfigTests
{
  [Test]
  public void EncryptionServiceConfig_GivenConstructed_ShouldDefaultEnabled() =>
    Assert.IsFalse(new RnEncryptionConfig().Enabled);

  [Test]
  public void EncryptionServiceConfig_GivenConstructed_ShouldDefaultKey() =>
    Assert.AreEqual(string.Empty, new RnEncryptionConfig().Key);

  [Test]
  public void EncryptionServiceConfig_GivenConstructed_ShouldDefaultIV() =>
    Assert.AreEqual(string.Empty, new RnEncryptionConfig().IV);

  [Test]
  public void EncryptionServiceConfig_GivenConstructed_ShouldDefaultLogDecryptInput() =>
    Assert.IsFalse(new RnEncryptionConfig().LogDecryptInput);

  [Test]
  public void EncryptionServiceConfig_GivenConstructed_ShouldDefaultLoggingEnabled() =>
    Assert.IsFalse(new RnEncryptionConfig().LoggingEnabled);

  [Test]
  public void EncryptionServiceConfig_GivenConfigKey_ShouldReturnExpectedValue() =>
    Assert.AreEqual("Rn.Encryption", RnEncryptionConfig.ConfigKey);
}
