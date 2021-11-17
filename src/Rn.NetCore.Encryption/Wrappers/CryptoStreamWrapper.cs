using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Rn.NetCore.Encryption.Wrappers
{
  public interface IStream : IDisposable, IAsyncDisposable
  {
    bool CanRead { get; }
    bool CanSeek { get; }
    bool CanTimeout { get; }
    bool CanWrite { get; }
    long Length { get; }
    long Position { get; set; }
    int ReadTimeout { get; set; }
    int WriteTimeout { get; set; }

    Task CopyToAsync(Stream destination);
    Task CopyToAsync(Stream destination, int bufferSize);
    Task CopyToAsync(Stream destination, CancellationToken cancellationToken);
    Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken);
    void CopyTo(Stream destination);
    void CopyTo(Stream destination, int bufferSize);
    void Close();
    void Flush();
    Task FlushAsync();
    Task FlushAsync(CancellationToken cancellationToken);
    IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state);
    int EndRead(IAsyncResult asyncResult);
    Task<int> ReadAsync(byte[] buffer, int offset, int count);
    Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);
    ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state);
    void EndWrite(IAsyncResult asyncResult);
    Task WriteAsync(byte[] buffer, int offset, int count);
    Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);
    ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
    long Seek(long offset, SeekOrigin origin);
    void SetLength(long value);
    int Read(byte[] buffer, int offset, int count);
    int Read(Span<byte> buffer);
    int ReadByte();
    void Write(byte[] buffer, int offset, int count);
    void Write(ReadOnlySpan<byte> buffer);
    void WriteByte(byte value);
  }

  public interface ICryptoStream : IStream
  {
    bool HasFlushedFinalBlock { get; }

    void FlushFinalBlock();
    ValueTask FlushFinalBlockAsync(CancellationToken cancellationToken = default);
    void Clear();
  }

  public class CryptoStreamWrapper : ICryptoStream
  {
    // Constructors
    private readonly CryptoStream _cryptoStream;

    public CryptoStreamWrapper(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
    {
      _cryptoStream = new CryptoStream(stream, transform, mode);
    }

    public CryptoStreamWrapper(Stream stream, ICryptoTransform transform, CryptoStreamMode mode, bool leaveOpen)
    {
      _cryptoStream = new CryptoStream(stream, transform, mode, leaveOpen);
    }

    public CryptoStreamWrapper(CryptoStream cryptoStream)
    {
      _cryptoStream = cryptoStream;
    }


    // Interface properties
    public bool CanRead => _cryptoStream.CanRead;
    public bool CanSeek => _cryptoStream.CanSeek;
    public bool CanTimeout => _cryptoStream.CanTimeout;
    public bool CanWrite => _cryptoStream.CanWrite;
    public long Length => _cryptoStream.Length;
    public bool HasFlushedFinalBlock => _cryptoStream.HasFlushedFinalBlock;

    public long Position
    {
      get => _cryptoStream.Position;
      set => _cryptoStream.Position = value;
    }

    public int ReadTimeout
    {
      get => _cryptoStream.ReadTimeout;
      set => _cryptoStream.ReadTimeout = value;
    }

    public int WriteTimeout
    {
      get => _cryptoStream.WriteTimeout;
      set => _cryptoStream.WriteTimeout = value;
    }


    // Interface methods
    public void Dispose()
      => _cryptoStream.Dispose();

    public ValueTask DisposeAsync()
      => _cryptoStream.DisposeAsync();

    public async Task CopyToAsync(Stream destination)
      => await _cryptoStream.CopyToAsync(destination);

    public async Task CopyToAsync(Stream destination, int bufferSize)
      => await _cryptoStream.CopyToAsync(destination, bufferSize);

    public async Task CopyToAsync(Stream destination, CancellationToken cancellationToken)
      => await _cryptoStream.CopyToAsync(destination, cancellationToken);

    public async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
      => await _cryptoStream.CopyToAsync(destination, bufferSize, cancellationToken);

    public void CopyTo(Stream destination)
      => _cryptoStream.CopyTo(destination);

    public void CopyTo(Stream destination, int bufferSize)
      => _cryptoStream.CopyTo(destination, bufferSize);

    public void Close()
      => _cryptoStream.Close();

    public void Flush()
      => _cryptoStream.Flush();

    public async Task FlushAsync()
      => await _cryptoStream.FlushAsync();

    public async Task FlushAsync(CancellationToken cancellationToken)
      => await _cryptoStream.FlushAsync(cancellationToken);

    public IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
      => _cryptoStream.BeginRead(buffer, offset, count, callback, state);

    public int EndRead(IAsyncResult asyncResult)
      => _cryptoStream.EndRead(asyncResult);

    public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
      => await _cryptoStream.ReadAsync(buffer, offset, count);

    public async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
      => await _cryptoStream.ReadAsync(buffer, offset, count, cancellationToken);

    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
      => _cryptoStream.ReadAsync(buffer, cancellationToken);

    public IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
      => _cryptoStream.BeginWrite(buffer, offset, count, callback, state);

    public void EndWrite(IAsyncResult asyncResult)
      => _cryptoStream.EndRead(asyncResult);

    public async Task WriteAsync(byte[] buffer, int offset, int count)
      => await _cryptoStream.WriteAsync(buffer, offset, count);

    public async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
      => await _cryptoStream.WriteAsync(buffer, offset, count, cancellationToken);

    public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
      => _cryptoStream.WriteAsync(buffer, cancellationToken);

    public long Seek(long offset, SeekOrigin origin)
      => _cryptoStream.Seek(offset, origin);

    public void SetLength(long value)
      => _cryptoStream.SetLength(value);

    public int Read(byte[] buffer, int offset, int count)
      => _cryptoStream.Read(buffer, offset, count);

    public int Read(Span<byte> buffer)
      => _cryptoStream.Read(buffer);

    public int ReadByte()
      => _cryptoStream.ReadByte();

    public void Write(byte[] buffer, int offset, int count)
      => _cryptoStream.Write(buffer, offset, count);

    public void Write(ReadOnlySpan<byte> buffer)
      => _cryptoStream.Write(buffer);

    public void WriteByte(byte value)
      => _cryptoStream.WriteByte(value);

    public void FlushFinalBlock()
      => _cryptoStream.FlushFinalBlock();

    public ValueTask FlushFinalBlockAsync(CancellationToken cancellationToken = default)
      => _cryptoStream.FlushFinalBlockAsync(cancellationToken);

    public void Clear()
      => _cryptoStream.Clear();
  }
}
