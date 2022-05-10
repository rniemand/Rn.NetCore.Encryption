using System;
using System.IO;
using System.Security.Cryptography;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Encryption.T1.Tests.TestSupport;
using Rn.NetCore.Encryption.Wrappers;

namespace Rn.NetCore.Encryption.T1.Tests.EncryptionServiceTests;

[TestFixture]
public class EncryptTests
{
  private const string EncryptionKey = "rigeU7mR2zA=";
  private static readonly byte[] KeyBytes = { 174, 24, 30, 83, 12, 25, 215, 48 };
  private const string EncryptionIV = "5ffasfasg4w/stkaYXm/+Mi4Aw=";
  private static readonly byte[] IVBytes = { 231, 254, 102, 26, 219, 118, 196, 80, 127, 70 };
  private const string InputValue = "Hello World";
  private static readonly byte[] InputBytes = { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100 };
  private static readonly byte[] MemStreamBytes = { 36, 102, 127, 185, 160, 139, 13, 116, 241, 139, 29, 233, 30, 47, 71, 190 };

  [Test]
  public void Encrypt_GivenDisabled_ShouldReturnNull()
  {
    // arrange
    var config = new EncryptionServiceConfigBuilder().BuildWithDefaults(false);

    var encryptionService = TestHelper.GetService(
      config: config
    );

    // act
    var encrypted = encryptionService.Encrypt("input");

    // assert
    Assert.IsNull(encrypted);
  }

  [Test]
  public void Encrypt_GivenEmptyString_ShouldReturnNull()
  {
    // arrange
    var config = new EncryptionServiceConfigBuilder().BuildWithDefaults();

    var encryptionService = TestHelper.GetService(
      config: config
    );

    // act
    var encrypted = encryptionService.Encrypt("");

    // assert
    Assert.IsNull(encrypted);
  }

  [Test]
  public void Encrypt_GivenString_ShouldRunEncryptionLogic()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();
    var cryptoTransform = Substitute.For<ICryptoTransform>();
    var cryptoStream = Substitute.For<ICryptoStream>();

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .Build();

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);
    encryptionHelper.CreateEncryptor(KeyBytes, IVBytes).Returns(cryptoTransform);

    encryptionHelper
      .CreateCryptoStream(Arg.Any<Stream>(), cryptoTransform, CryptoStreamMode.Write)
      .Returns(cryptoStream);

    encryptionHelper
      .When(x => x.CreateCryptoStream(Arg.Any<Stream>(), cryptoTransform, CryptoStreamMode.Write))
      .Do(info =>
      {
        info.Arg<Stream>().Write(MemStreamBytes);
        info.Arg<Stream>().Flush();
      });

    encryptionHelper.GetBytes(InputValue).Returns(InputBytes);
    encryptionHelper.ToBase64String(Arg.Any<byte[]>()).Returns("encrypted");

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper
    );

    // act
    var encrypted = encryptionService.Encrypt(InputValue);

    // assert
    cryptoStream.Received(1).Write(InputBytes, 0, InputBytes.Length);
    cryptoStream.Received(1).FlushFinalBlock();
    cryptoStream.Received(1).Close();
    Assert.AreEqual("encrypted", encrypted);
  }

  [Test]
  public void Encrypt_GivenFails_ShouldLog()
  {
    // arrange
    var logger = Substitute.For<ILoggerAdapter<EncryptionService>>();
    var encryptionHelper = Substitute.For<IEncryptionHelper>();
    var ex = new Exception("Whoops");

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithLoggingEnabled(true)
      .Build();

    encryptionHelper
      .CreateEncryptor(Arg.Any<byte[]>(), Arg.Any<byte[]>())
      .Throws(ex);

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper,
      logger: logger
    );

    // act
    encryptionService.Encrypt(InputValue);

    // assert
    logger.Received(1).LogError(
      "An unexpected exception of type {exType} was thrown in {method}. {exMessage}. | {exStack}",
      ex.GetType().Name,
      Arg.Any<string>(),
      ex.Message,
      ex.HumanStackTrace()
    );
  }

  [Test]
  public void Encrypt_GivenFailsLoggingDisabled_ShouldNotLog()
  {
    // arrange
    var logger = Substitute.For<ILoggerAdapter<EncryptionService>>();
    var encryptionHelper = Substitute.For<IEncryptionHelper>();
    var ex = new Exception("Whoops");

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithLoggingEnabled(false)
      .Build();

    encryptionHelper
      .CreateEncryptor(Arg.Any<byte[]>(), Arg.Any<byte[]>())
      .Throws(ex);

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper,
      logger: logger
    );

    // act
    encryptionService.Encrypt(InputValue);

    // assert
    logger.DidNotReceive().LogError(
      "An unexpected exception of type {exType} was thrown in {method}. {exMessage}. | {exStack}",
      ex.GetType().Name,
      Arg.Any<string>(),
      ex.Message,
      ex.HumanStackTrace()
    );
  }

  [Test]
  public void Encrypt_GivenFails_ShouldReturnNull()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();
    var ex = new Exception("Whoops");

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithLoggingEnabled(false)
      .Build();

    encryptionHelper
      .CreateEncryptor(Arg.Any<byte[]>(), Arg.Any<byte[]>())
      .Throws(ex);

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper
    );

    // act
    var output = encryptionService.Encrypt(InputValue);

    // assert
    Assert.IsNull(output);
  }
}
