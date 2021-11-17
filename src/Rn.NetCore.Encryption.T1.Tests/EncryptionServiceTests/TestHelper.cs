using NSubstitute;
using Rn.NetCore.Common.Configuration;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Encryption.T1.Tests.TestSupport;

namespace Rn.NetCore.Encryption.T1.Tests.EncryptionServiceTests
{
  public static class TestHelper
  {
    public static EncryptionService GetService(
      ILoggerAdapter<EncryptionService> logger = null,
      IEncryptionHelper encryptionHelper = null,
      ICommonConfigProvider configProvider = null,
      EncryptionServiceConfig config = null)
    {
      return new EncryptionService(
        logger ?? Substitute.For<ILoggerAdapter<EncryptionService>>(),
        encryptionHelper ?? Substitute.For<IEncryptionHelper>(),
        GetConfigProvider(configProvider, config)
      );
    }

    public static ICommonConfigProvider GetConfigProvider(
      ICommonConfigProvider configProvider = null,
      EncryptionServiceConfig config = null)
    {
      if (configProvider != null)
        return configProvider;

      config ??= new EncryptionServiceConfigBuilder().BuildWithDefaults();
      configProvider = Substitute.For<ICommonConfigProvider>();
      configProvider.GetEncryptionServiceConfig().Returns(config);

      return configProvider;
    }
  }
}
