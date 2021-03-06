using System;
using System.IO;
using System.Security.Cryptography;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Encryption.Configuration;

namespace Rn.NetCore.Encryption;

public interface IEncryptionService
{
  string? Encrypt(string plainText);
  string? Decrypt(string encryptedText);
  bool CanDecrypt(string encryptedText);
}

public class EncryptionService : IEncryptionService
{
  private readonly ILoggerAdapter<EncryptionService> _logger;
  private readonly IEncryptionHelper _encryptionHelper;
  private readonly RnEncryptionConfig _config;

  private readonly byte[] _keyBytes;
  private readonly byte[] _ivBytes;

  public EncryptionService(
    ILoggerAdapter<EncryptionService> logger,
    IEncryptionHelper encryptionHelper,
    RnEncryptionConfig config)
  {
    _logger = logger;
    _encryptionHelper = encryptionHelper;
    _config = config;

    _keyBytes = _encryptionHelper.FromBase64String(_config.Key);
    _ivBytes = _encryptionHelper.FromBase64String(_config.IV);

    // Check if we need to warn about potential bad config values
    if (_config.LoggingEnabled && _config.LogDecryptInput)
    {
      _logger.LogError(
        "Encryption input value logging has been enabled, " +
        "this is intended only for troubleshooting purposes and should " +
        "be disabled once completed!");
    }
  }

  public string? Encrypt(string plainText)
  {
    if (!_config.Enabled || string.IsNullOrWhiteSpace(plainText))
      return null;

    try
    {
      using var mStream = new MemoryStream();
      var cryptoTransform = _encryptionHelper.CreateEncryptor(_keyBytes, _ivBytes);

      var cStream = _encryptionHelper.CreateCryptoStream(
        mStream,
        cryptoTransform,
        CryptoStreamMode.Write
      );

      var inputBytes = _encryptionHelper.GetBytes(plainText);
      cStream.Write(inputBytes, 0, inputBytes.Length);
      cStream.FlushFinalBlock();
      var returnBytes = mStream.ToArray();

      cStream.Close();
      mStream.Close();

      return _encryptionHelper.ToBase64String(returnBytes);
    }
    catch (Exception ex)
    {
      if (_config.LoggingEnabled)
        _logger.LogUnexpectedException(ex);

      return null;
    }
  }

  public string? Decrypt(string encryptedText)
  {
    // TODO: [REPLACE] (EncryptionService.Decrypt) Replace with better lib
    if (!_config.Enabled || string.IsNullOrWhiteSpace(encryptedText))
      return null;

    try
    {
      var encryptedBytes = _encryptionHelper.FromBase64String(encryptedText);
      using var msDecrypt = new MemoryStream(encryptedBytes);
      var cryptoTransform = _encryptionHelper.CreateDecryptor(_keyBytes, _ivBytes);

      var csDecrypt = _encryptionHelper.CreateCryptoStream(
        msDecrypt,
        cryptoTransform,
        CryptoStreamMode.Read);

      var fromEncrypt = new byte[encryptedBytes.Length];
      csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

      var bufferString = _encryptionHelper.GetString(fromEncrypt);

      return bufferString.Replace("\0", "");
    }
    catch (Exception ex)
    {
      if (!_config.LoggingEnabled)
        return null;

      if (_config.LogDecryptInput)
      {
        _logger.LogError(ex,
          "Unable to decrypt: {i}. {s}",
          encryptedText,
          ex.HumanStackTrace());
      }
      else
      {
        _logger.LogUnexpectedException(ex);
      }

      return null;
    }
  }

  public bool CanDecrypt(string encryptedText)
  {
    if (!_config.Enabled || string.IsNullOrWhiteSpace(encryptedText))
      return false;

    return Decrypt(encryptedText) != null;
  }
}
