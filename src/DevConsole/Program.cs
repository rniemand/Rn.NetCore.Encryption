using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Encryption;
using Rn.NetCore.Encryption.Providers;

namespace DevConsole;

internal class Program
{
  private static IServiceProvider _services;
  private static ILoggerAdapter<Program> _logger;

  static void Main(string[] args)
  {
    ConfigureDI();

    var encryptionService = _services.GetRequiredService<IEncryptionService>();

    var encrypted = encryptionService.Encrypt("Hello World");
    var decrypted = encryptionService.Decrypt(encrypted);

    Console.WriteLine("Fin.");
  }

  // DI related methods
  private static void ConfigureDI()
  {
    var services = new ServiceCollection();

    var config = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
      .Build();

    services
      // Configuration
      .AddSingleton<IConfiguration>(config)
      .AddSingleton<IEncryptionConfigProvider, EncryptionConfigProvider>()

      // Logging
      .AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>))
      .AddLogging(loggingBuilder =>
      {
        // configure Logging with NLog
        loggingBuilder.ClearProviders();
        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
        loggingBuilder.AddNLog(config);
      })

      // Encryption
      .AddSingleton<IEncryptionService, EncryptionService>()
      .AddSingleton<IEncryptionHelper, EncryptionHelper>();

    _services = services.BuildServiceProvider();
    _logger = _services.GetService<ILoggerAdapter<Program>>();
  }
}
