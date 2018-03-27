using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Utils
{
    public interface IReadWriteAsyncStream
    {
        string ContentType { get; }
        string FileName { get; }
        Action<Stream, HttpContent, TransportContext> OnOutputStreamAvailable { get; }
    }
}