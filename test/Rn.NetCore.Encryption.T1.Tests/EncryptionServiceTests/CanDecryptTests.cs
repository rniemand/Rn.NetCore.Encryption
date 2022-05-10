using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Rn.NetCore.Encryption.T1.Tests.TestSupport;

namespace Rn.NetCore.Encryption.T1.Tests.EncryptionServiceTests;

[TestFixture]
public class CanDecryptTests
{
  private const string EncryptionKey = "rigeU7mR2zA=";
  private static readonly byte[] KeyBytes = { 174, 24, 30, 83, 12, 25, 215, 48 };
  private const string EncryptionIV = "5ffasfasg4w/stkaYXm/+Mi4Aw=";
  private static readonly byte[] IVBytes = { 231, 254, 102, 26, 219, 118, 196, 80, 127, 70 };

  [Test]
  public void CanDecrypt_GivenEncryptionServiceDisabled_ShouldReturnFalse()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .WithEnabled(false)
      .Build();

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper
    );

    // act
    var canDecrypt = encryptionService.CanDecrypt("bob");

    // assert
    Assert.IsFalse(canDecrypt);
  }

  [TestCase("")]
  [TestCase(null)]
  [TestCase("   ")]
  public void CanDecrypt_GivenInvalidInput_ShouldReturnFalse(string input)
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .WithEnabled(true)
      .Build();

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper
    );

    // act
    var canDecrypt = encryptionService.CanDecrypt(input);

    // assert
    Assert.IsFalse(canDecrypt);
  }

  [Test]
  public void CanDecrypt_GivenValidInput_ShouldCallDecrypt()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .WithEnabled(true)
      .Build();

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper
    );

    // act
    encryptionService.CanDecrypt("bob");

    // assert
    encryptionHelper.Received(1).FromBase64String("bob");
  }

  [Test]
  public void CanDecrypt_GivenDecryptFails_ShouldReturnFalse()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);
    encryptionHelper.FromBase64String("bob").Throws(new Exception());

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .WithEnabled(true)
      .Build();

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper
    );

    // act
    var canDecrypt = encryptionService.CanDecrypt("bob");

    // assert
    Assert.IsFalse(canDecrypt);
  }

  [Test]
  public void CanDecrypt_GivenDecryptPasses_ShouldReturnTrue()
  {
    // arrange
    var encryptionHelper = Substitute.For<IEncryptionHelper>();

    encryptionHelper.FromBase64String(EncryptionKey).Returns(KeyBytes);
    encryptionHelper.FromBase64String(EncryptionIV).Returns(IVBytes);

    var config = new EncryptionServiceConfigBuilder()
      .WithDefaults()
      .WithKey(EncryptionKey)
      .WithIV(EncryptionIV)
      .WithEnabled(true)
      .Build();

    var encryptionService = TestHelper.GetService(
      config: config,
      encryptionHelper: encryptionHelper
    );

    // act
    var canDecrypt = encryptionService.CanDecrypt("bob");

    // assert
    Assert.IsTrue(canDecrypt);
  }
}
