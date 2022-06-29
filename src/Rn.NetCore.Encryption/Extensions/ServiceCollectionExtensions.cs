using Microsoft.Extensions.DependencyInjection;

namespace Rn.NetCore.Encryption.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddRnEncryption(this IServiceCollection services)
  {
    return services;
  }
}
