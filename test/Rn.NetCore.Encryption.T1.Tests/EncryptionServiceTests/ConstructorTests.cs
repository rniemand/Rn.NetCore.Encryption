using NSubstitute;
using NUnit.Framework;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Encryption.T1.Tests.TestSupport;

namespace Rn.NetCore.Encryption.T1.Tests.EncryptionServiceTests
{
  [TestFixture]
  public class ConstructorTests
  {
    private const string EncryptionKey = "rigeU7mR2zA=";
    private const string EncryptionIV = "5ffasfasg4w/stkaYXm/+Mi4Aw=";
    
    [Test]
    public void EncryptionService_GivenConstructed_ShouldCallGetEncryptionServiceConfig()
    {
      // arrange
      var configProvider = TestHelper.GetConfigProvider();
      
      // act
      TestHelper.GetService(
        configProvider: configProvider
      );

      // assert
      configProvider.Received(1).GetEncryptionServiceConfig();
    }

    [Test]
    public void EncryptionService_GivenEnabled_ShouldSetKeyBytes()
    {
      // arrange
      var encryptionHelper = Substitute.For<IEncryptionHelper>();

      var config = new EncryptionServiceConfigBuilder()
        .WithDefaults()
        .WithKey(EncryptionKey)
        .Build();

      // act
      TestHelper.GetService(
        encryptionHelper: encryptionHelper,
        config: config
      );

      // assert
      encryptionHelper.Received(1).FromBase64String(EncryptionKey);
    }

    [Test]
    public void EncryptionService_GivenEnabled_ShouldSetIVBytes()
    {
      // arrange
      var encryptionHelper = Substitute.For<IEncryptionHelper>();

      var config = new EncryptionServiceConfigBuilder()
        .WithDefaults()
        .WithIV(EncryptionIV)
        .Build();

      // act
      TestHelper.GetService(
        encryptionHelper: encryptionHelper,
        config: config
      );

      // assert
      encryptionHelper.Received(1).FromBase64String(EncryptionIV);
    }

    [Test]
    public void EncryptionService_GivenInputLoggingEnabled_ShouldLogError()
    {
      // arrange
      var logger = Substitute.For<ILoggerAdapter<EncryptionService>>();

      var config = new EncryptionServiceConfigBuilder()
        .WithDefaults()
        .WithLogDecryptInput(true)
        .WithLoggingEnabled(true)
        .Build();

      // act
      TestHelper.GetService(
        config: config,
        logger: logger
      );

      // assert
      logger.Received(1).LogError(
        "Encryption input value logging has been enabled, " +
        "this is intended only for troubleshooting purposes and should " +
        "be disabled once completed!"
      );
    }
  }
}
