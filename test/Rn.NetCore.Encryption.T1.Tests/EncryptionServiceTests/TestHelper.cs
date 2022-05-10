using NSubstitute;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Encryption.Configuration;
using Rn.NetCore.Encryption.Providers;
using Rn.NetCore.Encryption.T1.Tests.TestSupport;

namespace Rn.NetCore.Encryption.T1.Tests.EncryptionServiceTests;

public static class TestHelper
{
  public static EncryptionService GetService(
    ILoggerAdapter<EncryptionService> logger = null,
    IEncryptionHelper encryptionHelper = null,
    IEncryptionConfigProvider configProvider = null,
    EncryptionServiceConfig config = null)
  {
    return new EncryptionService(
      logger ?? Substitute.For<ILoggerAdapter<EncryptionService>>(),
      encryptionHelper ?? Substitute.For<IEncryptionHelper>(),
      GetConfigProvider(configProvider, config)
    );
  }

  public static IEncryptionConfigProvider GetConfigProvider(
    IEncryptionConfigProvider configProvider = null,
    EncryptionServiceConfig config = null)
  {
    if (configProvider != null)
      return configProvider;

    config ??= new EncryptionServiceConfigBuilder().BuildWithDefaults();
    configProvider = Substitute.For<IEncryptionConfigProvider>();
    configProvider.Provide().Returns(config);

    return configProvider;
  }
}
