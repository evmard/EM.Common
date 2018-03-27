using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Utils
{
    public class ReadWriteAsyncStream : Stream, IReadWriteAsyncStream
    {
        public ReadWriteAsyncStream(int bufferSize = 65536)
        {
            _bufferSize = bufferSize;
            _inputBuffer = new byte[_bufferSize];
            _outputBuffer = new byte[_bufferSize];
            _inputStreamOpen = true;
            OutputStreamError = null;
        }

        private readonly int _bufferSize = 65536;
        private readonly byte[] _inputBuffer;
        private int _dataSize;

        private readonly byte[] _outputBuffer;
        private readonly object _locker = new object();

        private readonly ManualResetEvent _onDataReady = new ManualResetEvent(false);

        public string OutputStreamError { get; private set; }
        
        private bool _inputStreamOpen;

        public string ContentType { get; set; }
        public string FileName { get; set; } 
            
        public Action<Stream, HttpContent, TransportContext> OnOutputStreamAvailable
        {
            get { return WriteOutputData; }
        }

        public override bool CanRead { get; } = false;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = true;
        public override long Length { get; } = 0;
        public override long Position { get; set; }

        public async void WriteOutputData(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                bool inputStreamOpen = true;

                while (inputStreamOpen)
                {
                    _onDataReady.WaitOne();
                    int dataSize;
                    lock (_locker)
                    {
                        _onDataReady.Reset();
                        inputStreamOpen = _inputStreamOpen;
                        if (_dataSize == 0)
                            continue;
                        
                        dataSize = _dataSize;
                        Buffer.BlockCopy(_inputBuffer, 0, _outputBuffer, 0, dataSize);
                        _dataSize = 0;
                    }

                    await outputStream.WriteAsync(_outputBuffer, 0, dataSize);
                }

            }
            catch (Exception ex)
            {
                OutputStreamError = ex.Message;
                Close();
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var bytesWritten = 0;
            while (bytesWritten < count)
            {
                SpinWait.SpinUntil(() => (_bufferSize - _dataSize) > 0);
                lock (_locker)
                {
                    if (!_inputStreamOpen)
                    {
                        var msg = "Input stream is closed ";
                        if (OutputStreamError != null)
                            msg = string.Format("{0}\n Output Stream Error: {1}", msg, OutputStreamError);

                        throw new IOException(msg);
                    }

                    var bytesToCopy = count - bytesWritten;
                    var remains = _bufferSize - _dataSize;
                    var willCopy = remains < bytesToCopy ? remains : bytesToCopy;
                    Buffer.BlockCopy(buffer, offset + bytesWritten, _inputBuffer, _dataSize, willCopy);
                    _dataSize += willCopy;
                    bytesWritten += willCopy;

                    if ((_bufferSize - _dataSize) == 0)
                        _onDataReady.Set();
                }
            }

            lock (_locker)
                _onDataReady.Set();
        }

        public override void Close()
        {
            base.Close();
            lock (_locker)
            {                
                _inputStreamOpen = false;
                _onDataReady.Set();
            }
        }
    }
}
