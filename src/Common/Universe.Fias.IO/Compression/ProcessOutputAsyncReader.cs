using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Universe.Types;

namespace Universe.Fias.IO.Compression
{
    /// <summary>
    ///     Считывает из выходного потока процесса без блокировки, данные до последнего байта.
    ///     Стандартный ассинхронный реадер, ситывает только строками, что приводит к тому, что когда процесс ждет ввода от пользователя
    ///     выведенная строка перед запросом ввода не считвается.
    /// </summary>
    public class ProcessOutputAsyncReader : DisposableObject
    {
        private readonly StringBuilder _sb;

        private readonly object _sync = new object();

        private byte[] _buffer;

        private Encoding _encoding;

        private bool _isDisposed;

        private IAsyncResult _last;

        private Process _process;

        private Stream _stream;

        private StringWriter _sw;

        public ProcessOutputAsyncReader(Process process, Stream stream, Encoding encoding)
        {
            _process = process;
            _stream = stream;
            _encoding = encoding;

            _sb = new StringBuilder();
            _sw = new StringWriter(_sb);
            _buffer = new byte[1024];
        }

        public void BeginRead()
        {
            if (_isDisposed)
            {
                return;
            }

            lock (_sync)
            {
                _last = _stream.BeginRead(_buffer, 0, _buffer.Length, BeginReadCallback, this);
            }
        }

        public string GetOutputString()
        {
            lock (_sync)
            {
                return _sb.ToString();
            }
        }

        public event EventHandler<EventArgs> Readed;

        public void StandardInputWriteLine(string command)
        {
            lock (_sync)
            {
                _sw.WriteLine();
                _sw.WriteLine($"Input: {command}");
            }
        }

        public void WaitForAllRead()
        {
            IAsyncResult prev = null;
            while (true)
            {
                IAsyncResult last;
                lock (_sync)
                {
                    last = _last;
                }

                if (last == null)
                {
                    return;
                }

                if (Equals(prev, last))
                {
                    return;
                }

                last.AsyncWaitHandle.WaitOne();
                prev = last;
            }
        }

        /// <summary>
        ///     <see href="https://msdn.microsoft.com/library/ms244737.aspx">CA1063: следует правильно реализовывать IDisposable</see>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            lock (_sync)
            {
                _isDisposed = true;
                if (disposing)
                {
                    _encoding = null;
                    _stream = null;
                    _process = null;
                    _sw?.Dispose();
                    _sw = null;
                    _buffer = null;
                }
            }
        }

        private void BeginReadCallback(IAsyncResult ar)
        {
            lock (_sync)
            {
                if (_isDisposed)
                {
                    return;
                }

                var numOfBytesRead = _stream.EndRead(ar);
                if (numOfBytesRead > 0)
                {
                    var chars = _encoding.GetChars(_buffer, 0, numOfBytesRead);
                    _sw.Write(chars, 0, chars.Length);

                    Console.Write(chars, 0, chars.Length);
                }

                if (numOfBytesRead == _buffer.Length)
                {
                    _last = _stream.BeginRead(_buffer, 0, _buffer.Length, BeginReadCallback, this);
                    return;
                }

                Readed?.Invoke(this, new EventArgs());

                if (_process.HasExited)
                {
                    return;
                }

                _last = _stream.BeginRead(_buffer, 0, _buffer.Length, BeginReadCallback, this);
            }
        }
    }
}