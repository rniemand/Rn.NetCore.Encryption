using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Rn.NetCore.Encryption;

public interface IEncryptionHelper
{
  ICryptoTransform CreateEncryptor(byte[] key, byte[] iv);
  ICryptoTransform CreateDecryptor(byte[] key, byte[] iv);
  byte[] FromBase64String(string s);
  string ToBase64String(byte[] inArray);
  byte[] GetBytes(string s);
  string GetString(byte[] bytes);
  ICryptoStream CreateCryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode);
  byte[] GetRandomBytes(int length);
}

[ExcludeFromCodeCoverage]
public class EncryptionHelper : IEncryptionHelper
{
  private readonly Random _random = new Random(DateTime.Now.Millisecond);

  public ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
    => new DESCryptoServiceProvider().CreateEncryptor(key, iv);

  public ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
    => new DESCryptoServiceProvider().CreateDecryptor(key, iv);

  public byte[] FromBase64String(string s)
    => Convert.FromBase64String(s);

  public string ToBase64String(byte[] inArray)
    => Convert.ToBase64String(inArray);

  public byte[] GetBytes(string s)
    => new ASCIIEncoding().GetBytes(s);

  public string GetString(byte[] bytes)
    => new ASCIIEncoding().GetString(bytes);

  public ICryptoStream CreateCryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
    => new CryptoStreamWrapper(stream, transform, mode);

  public byte[] GetRandomBytes(int length)
  {
    var output = new List<byte>();

    for (var i = 0; i < length; i++)
    {
      output.Add((byte)_random.Next(0, 255));
    }

    return output.ToArray();
  }
}
