using System;
using System.IO;
using System.Security.Cryptography;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Rn.NetCore.Common.Extensions;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Encryption.T1.Tests.TestSupport;
using Rn.NetCore.Encryption.Wrappers;

namespace Rn.NetCore.Encryption.T1.Tests.EncryptionServiceTests;

[TestFixture]
public class DecryptTests
{
  private const string EncryptionKey = "rigeU7mR2zA=";
  private static readonly byte[] KeyBytes = { 174, 24, 30, 83, 12, 25, 215, 48 };
  private const string EncryptionIV = "5ffasfasg4w/stkaYXm/+Mi4Aw=";
  private static readonly byte[] IVBytes = { 231, 254, 102, 26, 219, 118, 196, 80, 127, 70 };
  private const string InputValue = "Hello World";
  private static readonly byte[] InputBytes = { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100 };

  [Test]
  public void Decrypt_GivenDisabled_ShouldReturnNull()
  {
    // arrange
    var config = new EncryptionServiceConfigBuilder().BuildWithDefaults(false);
    var encryptionService = TestHelper.GetService(config: config);

    // act
    var encrypted = encryptionService.Decrypt("input");

    // assert
    Assert.IsNull(encrypted);
  }

  [Test]
  public void Decrypt_GivenEmptyString_ShouldReturnNull()
  {
    // arrange
    var config = new EncryptionServiceConfigBuilder().BuildWithDefaults();
    var encryptionService = TestHelper.GetService(config: config);

    // act
    var encrypted = encryptionService.Decrypt("");

    // assert
    Assert.IsNull(encrypted);
  }

  [Test]
  public void Decrypt_GivenString_ShouldRunEncryptionLogic()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();
    var cryptoTransform = Substitute.For<ICryptoTransform>();
    var cryptoStream = Substitute.For<ICryptoStream>();

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);
    encryptionHelper.FromBase64String(InputValue).Returns(InputBytes);
    encryptionHelper.CreateDecryptor(KeyBytes, IVBytes).Returns(cryptoTransform);

    encryptionHelper
      .CreateCryptoStream(Arg.Any<Stream>(), cryptoTransform, CryptoStreamMode.Read)
      .Returns(cryptoStream);

    cryptoStream
      .When(x => x.Read(Arg.Any<byte[]>(), 0, InputBytes.Length))
      .Do(info =>
      {
        var bytes = (info[0] as byte[]);
        for (var i = 0; i < InputBytes.Length; i++)
          // ReSharper disable once PossibleNullReferenceException
          bytes[i] = InputBytes[i];
      });

    encryptionHelper
      .GetString(Arg.Is<byte[]>(b => b.SameByteArray(InputBytes)))
      .Returns("decrypted");

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .Build();

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper);

    // act
    var decrypted = encryptionService.Decrypt(InputValue);

    // assert
    Assert.AreEqual("decrypted", decrypted);
  }

  [Test]
  public void Decrypt_GivenExceptionThrown_ShouldLog()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();
    var logger = Substitute.For<ILoggerAdapter<EncryptionService>>();
    var ex = new Exception("Whoops");

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);
    encryptionHelper.FromBase64String(InputValue).Throws(ex);

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .WithLoggingEnabled(true)
      .Build();

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper,
      logger: logger);

    // act
    encryptionService.Decrypt(InputValue);

    // assert
    logger.Received(1).LogError("An unexpected exception of type {exType} was thrown in {method}. {exMessage}. | {exStack}",
      ex.GetType().Name,
      Arg.Any<string>(),
      ex.Message,
      ex.HumanStackTrace());
  }

  [Test]
  public void Decrypt_GivenExceptionThrownWithInputLogging_ShouldLog()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();
    var logger = Substitute.For<ILoggerAdapter<EncryptionService>>();
    var ex = new Exception("Whoops");

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);
    encryptionHelper.FromBase64String(InputValue).Throws(ex);

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .WithLoggingEnabled(true)
      .WithLogDecryptInput(true)
      .Build();

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper,
      logger: logger);

    // act
    encryptionService.Decrypt(InputValue);

    // assert
    logger.Received(1).LogError(ex,
      "Unable to decrypt: {i}. {s}",
      InputValue,
      ex.HumanStackTrace());
  }

  [Test]
  public void Decrypt_GivenExceptionThrownAndLoggingDisabled_ShouldNotLog()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();
    var logger = Substitute.For<ILoggerAdapter<EncryptionService>>();
    var ex = new Exception("Whoops");

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);
    encryptionHelper.FromBase64String(InputValue).Throws(ex);

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .WithLoggingEnabled(false)
      .WithLogDecryptInput(true)
      .Build();

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper,
      logger: logger);

    // act
    encryptionService.Decrypt(InputValue);

    // assert
    logger.DidNotReceive().LogError(
      Arg.Any<Exception>(),
      Arg.Any<string>(),
      Arg.Any<string>(),
      Arg.Any<string>());
  }

  [Test]
  public void Decrypt_GivenExceptionThrown_ShouldReturnNull()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();
    var ex = new Exception("Whoops");

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);
    encryptionHelper.FromBase64String(InputValue).Throws(ex);

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .WithLoggingEnabled(false)
      .WithLogDecryptInput(true)
      .Build();

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper);

    // act
    var output = encryptionService.Decrypt(InputValue);

    // assert
    Assert.IsNull(output);
  }
}
