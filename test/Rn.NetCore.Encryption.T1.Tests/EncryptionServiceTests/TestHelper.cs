using NSubstitute;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Encryption.Configuration;

namespace Rn.NetCore.Encryption.T1.Tests.EncryptionServiceTests;

public static class TestHelper
{
  public static EncryptionService GetService(
    ILoggerAdapter<EncryptionService> logger = null,
    IEncryptionHelper encryptionHelper = null,
    EncryptionServiceConfig config = null)
  {
    return new EncryptionService(
      logger ?? Substitute.For<ILoggerAdapter<EncryptionService>>(),
      encryptionHelper ?? Substitute.For<IEncryptionHelper>(),
      config ?? new EncryptionServiceConfig()
    );
  }
}
