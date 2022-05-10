# Rn.NetCore.Encryption
Simple encryption utility for use with your applications, depends on [Rn.NetCore.Common](https://www.nuget.org/packages/Rn.NetCore.Common/).

- [Configuration](/docs/configuration/README.md) - covers required configuration
- [EncryptionServiceConfig](/docs/configuration/EncryptionServiceConfig.md) - core configuration stored in the `appsettings.json` file.

## Usage
Using this package is as simple as adding the following configuration to your projects `appsettings.json` file:

```json
{
  "Rn.Encryption": {
    "enabled": true,
    "key": "rhgeU6mR1zA=",
    "iv": "5/5mGtt2xF...Mi4Aw="
  }
}
```

Injecting `IEncryptionService` where you need to use it and call the `.Encrypt()` and `.Decrypt()` methods.

<!--(Rn.BuildScriptHelper){
	"version": "1.0.106",
	"replace": false
}(END)-->