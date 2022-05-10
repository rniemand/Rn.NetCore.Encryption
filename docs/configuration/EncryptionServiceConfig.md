[Home](/README.md) / [Configuration](/docs/configuration/README.md) / EncryptionServiceConfig

# EncryptionServiceConfig
Core configuration for the the **Rn.NetCore.Encryption** package.

These settings should be present in `IConfiguration` for the package to see it.

```json
{
  "Rn.Encryption": {
    "enabled": true,
    "key": "rhgeU6mR1zA=",
    "iv": "5/5mGtt2xF...Mi4Aw=",
    "loggingEnabled": true,
    "logDecryptInput": false
  }
}
```

Details on each option is listed below.

| Property | Type | Required | Default | Notes |
| --- | --- | ---- | ---- | --- |
| `enabled` | `bool` | optional | `false` | Enables the usage of the encryption classes |
| `key` | `string` | required | - | Encryption key to use. |
| `iv` | `string` | required | - | Encryption `IV` to use. |
| `loggingEnabled` | `bool` | optional | `false` | Enables logging for all encryption related methods. |
| `logDecryptInput` | `bool` | optional | `false` | Allows logging of decrypted input. **NOTE** this is intended for debugging. |
