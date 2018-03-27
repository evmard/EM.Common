using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Utils
{
    public class FileAsyncStream : IReadWriteAsyncStream
    {
        public FileAsyncStream(string filePath)
        {
            _filePath = filePath;
        }

        private readonly string _filePath;

        public Action<Stream, HttpContent, TransportContext> OnOutputStreamAvailable
        {
            get { return WriteOutputData; }
        }

        public string ContentType { get; set; }
        public string FileName { get; set; }

        public async void WriteOutputData(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                var buffer = new byte[65536];
                using (var file = File.Open(_filePath, FileMode.Open, FileAccess.Read))
                {
                    var length = (int)file.Length;
                    var bytesRead = 1;

                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = file.Read(buffer, 0, Math.Min(length, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        length -= bytesRead;
                    }
                }
            }
            catch
            {
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }
    }
}
