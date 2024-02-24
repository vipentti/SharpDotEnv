// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace SharpDotEnv.Extensions.Configuration.Tests;

public static class TestUtils
{
    public static Stream StringToStream(this string value, bool withBom = false)
    {
        var stream = new MemoryStream();
        var textWriter = new StreamWriter(stream, new System.Text.UTF8Encoding(withBom));
        textWriter.Write(value);
        textWriter.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    public static IFileProvider StringToFileProvider(this string value) =>
        new TestFileProvider(value);

    private sealed class TestFileProvider : IFileProvider
    {
        public string Data { get; }

        public TestFileProvider(string data)
        {
            Data = data;
        }

        public IDirectoryContents GetDirectoryContents(string subpath) =>
            throw new NotImplementedException();

        public IFileInfo GetFileInfo(string subpath) => new TestFile(Data);

        public IChangeToken Watch(string filter) => throw new NotImplementedException();
    }

    private sealed class TestFile : IFileInfo
    {
        public string Data { get; }

        public TestFile(string data)
        {
            Data = data;
        }

        public bool Exists => true;

        public long Length => throw new NotImplementedException();

        public string? PhysicalPath => null;

        public string Name => throw new NotImplementedException();

        public DateTimeOffset LastModified => throw new NotImplementedException();

        public bool IsDirectory => throw new NotImplementedException();

        public Stream CreateReadStream() => StringToStream(Data);
    }
}
