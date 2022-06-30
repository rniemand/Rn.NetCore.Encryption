using System;
using Microsoft.Extensions.DependencyInjection;
using Rn.NetCore.Encryption;

namespace DevConsole;

internal class Program
{
  private static void Main()
  {
    var encService = DIContainer.Services.GetRequiredService<IEncryptionService>();

    var encrypted = encService.Encrypt("Hello World!");
    var decrypted = encService.Decrypt(encrypted);

    Console.WriteLine(encrypted);
    Console.WriteLine(decrypted);
    Console.WriteLine("Fin.");
  }
}
