using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;
using Asv.Mavlink.Common;
using DotNext.Buffers;
using FluentAssertions;
using JetBrains.Annotations;
using R3;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Mavlink.Test;

[TestSubject(typeof(FtpServerEx))]
[TestSubject(typeof(FtpClientEx))]
public class FtpExComplexTest : ComplexTestBase<FtpClientEx, FtpServerEx>, IDisposable
{
    private MockFileSystem? _fileSystem;
    public FtpExComplexTest(ITestOutputHelper log) : base(log)
    {
        _cts = new CancellationTokenSource();
        _cts.Token.Register(() => _tcs.TrySetCanceled());
    }
    private readonly TaskCompletionSource<FileTransferProtocolPacket> _tcs = new();
    private readonly CancellationTokenSource _cts;

    

    private readonly MavlinkFtpServerConfig _serverConfig = new()
    {
        BurstReadChunkDelayMs = 0,
        NetworkId = 0
    };

    private readonly MavlinkFtpClientConfig _clientConfig = new()
    {
        TimeoutMs = 100,
        CommandAttemptCount = 5,
        TargetNetworkId = 0,
        BurstTimeoutMs = 100
    };

    private readonly MavlinkFtpServerExConfig _serverExConfig = new()
    {
        RootDirectory = AppDomain.CurrentDomain.BaseDirectory
    };

    private readonly MavlinkFtpServerConfig _serverFtpConfig = new()
    {
        NetworkId = 0,
        BurstReadChunkDelayMs = 100,
    };

    

    protected override FtpServerEx CreateServer(MavlinkIdentity identity, IMavlinkContext core)
    {
        _fileSystem = SetUpFileSystem(_serverExConfig.RootDirectory);
        return new FtpServerEx(new FtpServer(identity, _serverConfig, core), _serverExConfig, _fileSystem);
    }

    protected override FtpClientEx CreateClient(MavlinkClientIdentity identity, IMavlinkContext core)
    {
        return new FtpClientEx(new FtpClient(identity, _clientConfig, core));
    }

    [Theory]
    [InlineData(100, 239)]
    [InlineData(239, 239)]
    [InlineData(239, 10)]
    [InlineData(1024 * 10, 200)]
    public async Task DownloadFile_ToStream_Success(int size, byte partSize)
    {
        var server = Server; // to ensure that server is created and file system is created
        const string fileName = "test.txt";
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var filePath = _fileSystem.Path.Combine(_serverExConfig.RootDirectory, fileName);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        var originContent = new byte[size];
        new Random().NextBytes(originContent);
        _fileSystem.AddFile(filePath, new MockFileData(originContent));

        using var streamToSave = new MemoryStream(size);

        // Act
        var progress = 0.0;
        await Client.DownloadFile(fileName, streamToSave, new CallbackProgress<double>(x => progress = x), partSize,
            _cts.Token);
        // Assert

        streamToSave.Seek(0, SeekOrigin.Begin);
        Assert.Equal(1, progress);
        Assert.Equal(originContent.Length, streamToSave.Length);
        foreach (var b in originContent)
        {
            Assert.Equal(streamToSave.ReadByte(), b);
        }
    }

    [Theory]
    [InlineData(100, 239)]
    [InlineData(239, 239)]
    [InlineData(239, 10)]
    [InlineData(1024 * 10, 200)]
    public async Task DownloadFile_ToBufferWriter_Success(int size, byte partSize)
    {
        var server = Server; // to ensure that server is created and file system is created
        const string fileName = "test.txt";
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var filePath = _fileSystem.Path.Combine(_serverExConfig.RootDirectory, fileName);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        var originContent = new byte[size];
        new Random().NextBytes(originContent);
        _fileSystem.AddFile(filePath, new MockFileData(originContent));

        var buffer = new ArrayBufferWriter<byte>(size);
        var progress = 0.0;
        // Act
        await Client.DownloadFile(fileName, buffer, new CallbackProgress<double>(x => { progress = x; }), partSize,
            _cts.Token);
        // Assert

        Assert.Equal(originContent.Length, buffer.WrittenCount);
        Assert.Equal(1, progress);
        for (var index = 0; index < originContent.Length; index++)
        {
            Assert.Equal(originContent[index], buffer.WrittenSpan[index]);
        }
    }

    [Theory]
    [InlineData(100, 239)]
    [InlineData(239, 239)]
    [InlineData(239, 10)]
    [InlineData(1024 * 10, 200)]
    public async Task BurstDownloadFile_ToStream_Success(int size, byte partSize)
    {
        // Arrange
        var server = Server; // to ensure that server is created and file system is created
        const string fileName = "test.txt";
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var filePath = _fileSystem.Path.Combine(_serverExConfig.RootDirectory, fileName);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        var originContent = new byte[size];
        new Random().NextBytes(originContent);
        _fileSystem.AddFile(filePath, new MockFileData(originContent));

        using var streamToSave = new MemoryStream(size);
        var progress = 0.0;
        // Act
        await Client.BurstDownloadFile(fileName, streamToSave, new CallbackProgress<double>(x => progress = x),
            partSize, _cts.Token);
        // Assert

        streamToSave.Seek(0, SeekOrigin.Begin);
        Assert.Equal(1, progress);
        Assert.Equal(originContent.Length, streamToSave.Length);
        foreach (var b in originContent)
        {
            Assert.Equal(streamToSave.ReadByte(), b);
        }
    }

    [Theory]
    [InlineData(100, 239)]
    [InlineData(239, 239)]
    [InlineData(239, 10)]
    [InlineData(1024 * 10, 200)]
    public async Task BurstDownloadFile_ToBufferWriter_Success(int size, byte partSize)
    {
        // Arrange
        var server = Server; // to ensure that server is created and file system is created
        const string fileName = "test.txt";
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var filePath = _fileSystem.Path.Combine(_serverExConfig.RootDirectory, fileName);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        var originContent = new byte[size];
        new Random().NextBytes(originContent);
        _fileSystem.AddFile(filePath, new MockFileData(originContent));

        var streamToSave = new ArrayBufferWriter<byte>();


        // Act
        var progress = 0.0;
        await Client.BurstDownloadFile(fileName, streamToSave, new CallbackProgress<double>(x => progress = x),
            partSize, _cts.Token);
        // Assert
        Assert.Equal(1, progress);
        Assert.Equal(originContent.Length, streamToSave.WrittenCount);
        for (var index = 0; index < originContent.Length; index++)
        {
            Assert.Equal(originContent[index], streamToSave.WrittenSpan[index]);
        }
    }

    [Theory]
    [InlineData("/")]
    [InlineData("")]
    public async Task Refresh_Success(string refreshPath)
    {
        // Arrange
        var server = Server; // to ensure that server is created and file system is created
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var localRoot = _fileSystem.Path.Combine(_serverExConfig.RootDirectory, "folder");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        _fileSystem.AddDirectory(localRoot);
        _fileSystem.AddEmptyFile(_fileSystem.Path.Combine(localRoot, "file1.txt"));
        _fileSystem.AddEmptyFile(_fileSystem.Path.Combine(localRoot, "file2.txt"));
        _fileSystem.AddEmptyFile(_fileSystem.Path.Combine(localRoot, "file3.txt"));


        var expectedFiles = new[]
        {
            $"{MavlinkFtpHelper.DirectorySeparator}",
            $"{MavlinkFtpHelper.DirectorySeparator}folder{MavlinkFtpHelper.DirectorySeparator}",
            $"{MavlinkFtpHelper.DirectorySeparator}folder{MavlinkFtpHelper.DirectorySeparator}file1.txt",
            $"{MavlinkFtpHelper.DirectorySeparator}folder{MavlinkFtpHelper.DirectorySeparator}file2.txt",
            $"{MavlinkFtpHelper.DirectorySeparator}folder{MavlinkFtpHelper.DirectorySeparator}file3.txt",
        };


        // Act
        await Client.Refresh(refreshPath, true, _cts.Token);

        // Assert
        expectedFiles.Should().Equal(Client.Entries.Keys);
    }

    private static MockFileSystem SetUpFileSystem(string root)
    {
        var mockFileCfg = new MockFileSystemOptions
        {
            CurrentDirectory = root
        };
        var fileSystem = new MockFileSystem(mockFileCfg);
        fileSystem.AddDirectory(mockFileCfg.CurrentDirectory);

        return fileSystem;
    }

    [Theory]
    [InlineData(100, 239)]
    [InlineData(239, 239)]
    [InlineData(239, 10)]
    [InlineData(1024 * 10, 200)]
    public async Task DownloadFile_ToStream_Cancel_Throws(int size, byte partSize)
    {
        // Arrange
        var server = Server;
        await _cts.CancelAsync();
        const string fileName = "test.txt";
        var filePath = _fileSystem.Path.Combine(_serverExConfig.RootDirectory, fileName);
        var originContent = new byte[size];
        new Random().NextBytes(originContent);
        _fileSystem.AddFile(filePath, new MockFileData(originContent));

        using var streamToSave = new MemoryStream(size);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await Client.DownloadFile(fileName, streamToSave, new CallbackProgress<double>(_ => { }), partSize,
                _cts.Token);
        });
    }

    [Theory]
    [InlineData(100, 239)]
    [InlineData(239, 239)]
    [InlineData(239, 10)]
    [InlineData(1024 * 10, 200)]
    public async Task DownloadFile_ToBufferWriter_Cancel_Throws(int size, byte partSize)
    {
        // Arrange
        var server = Server;
        await _cts.CancelAsync();
        const string fileName = "test.txt";
        var filePath = _fileSystem.Path.Combine(_serverExConfig.RootDirectory, fileName);
        var originContent = new byte[size];
        new Random().NextBytes(originContent);
        _fileSystem.AddFile(filePath, new MockFileData(originContent));

        var buffer = new ArrayBufferWriter<byte>();

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await Client.DownloadFile(fileName, buffer, new CallbackProgress<double>(_ => { }), partSize,
                _cts.Token);
        });
    }

    [Theory]
    [InlineData(100, 239)]
    [InlineData(239, 239)]
    [InlineData(239, 10)]
    [InlineData(1024 * 10, 200)]
    public async Task BurstDownloadFile_ToStream_Cancel_Throws(int size, byte partSize)
    {
        // Arrange
        var server = Server;
        await _cts.CancelAsync();
        const string fileName = "test.txt";
        var filePath = _fileSystem.Path.Combine(_serverExConfig.RootDirectory, fileName);
        var originContent = new byte[size];
        new Random().NextBytes(originContent);
        _fileSystem.AddFile(filePath, new MockFileData(originContent));

        using var streamToSave = new MemoryStream(size);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await Client.BurstDownloadFile(fileName, streamToSave, new CallbackProgress<double>(_ => { }), partSize,
                _cts.Token);
        });
    }

    [Theory]
    [InlineData(100, 239)]
    [InlineData(239, 239)]
    [InlineData(239, 10)]
    [InlineData(1024 * 10, 200)]
    public async Task BurstDownloadFile_ToBufferWriter_Cancel_Throws(int size, byte partSize)
    {
        // Arrange
        var server = Server;
        await _cts.CancelAsync();
        const string fileName = "test.txt";
        var filePath = _fileSystem.Path.Combine(_serverExConfig.RootDirectory, fileName);
        var originContent = new byte[size];
        new Random().NextBytes(originContent);
        _fileSystem.AddFile(filePath, new MockFileData(originContent));

        var buffer = new ArrayBufferWriter<byte>();

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await Client.BurstDownloadFile(fileName, buffer, new CallbackProgress<double>(_ => { }), partSize,
                _cts.Token);
        });
    }

    [Theory]
    [InlineData("/")]
    [InlineData("")]
    public async Task Refresh_Cancel_Throws(string refreshPath)
    {
        // Arrange
        var server = Server;
        await _cts.CancelAsync();
        var localRoot = _fileSystem.Path.Combine(_serverExConfig.RootDirectory, "folder");
        _fileSystem.AddDirectory(localRoot);
        _fileSystem.AddEmptyFile(_fileSystem.Path.Combine(localRoot, "file1.txt"));
        _fileSystem.AddEmptyFile(_fileSystem.Path.Combine(localRoot, "file2.txt"));
        _fileSystem.AddEmptyFile(_fileSystem.Path.Combine(localRoot, "file3.txt"));

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await Client.Refresh(refreshPath, true, _cts.Token);
        });
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}