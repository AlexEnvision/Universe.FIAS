using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Universe.Fias.XML.GAR.Converter.x64.Algorithms.AddressSystem.Loader
{
    /// <summary>
    /// Address system <seealso cref="System.Net.WebClient"/>.
    /// </summary>
    /// <seealso cref="System.Net.WebClient"/>
    public class AddrSysWebClient : WebClient
    {
        public event DownloadProgressChangedEventHandler DownloadProgressPartiallyChanged;

        /// <summary>
        /// Gets or sets the resume file range.
        /// </summary>
        /// <value>
        /// The resume file range.
        /// </value>
        public int? ResumeFileRange { get; set; }

        /// <summary>Returns a <see cref="T:System.Net.WebRequest"/> object for the specified resource.</summary>
        /// <returns>A new <see cref="T:System.Net.WebRequest"/> object for the specified resource.</returns>
        /// <param name="address">A <see cref="T:System.Uri"/> that identifies the resource to request.</param>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);

            var httpWebRequest = webRequest as HttpWebRequest;
            if (httpWebRequest != null && ResumeFileRange != null)
                httpWebRequest.AddRange(ResumeFileRange.Value);

            return webRequest;
        }

        /// <summary>
        ///     Загрузка файла порционно.
        /// </summary>
        /// <param name="address">Адрес (Конечная точка)</param>
        /// <param name="filepath">Путь созранения файла</param>
        /// <param name="timeout">Таймаут подключения в секундах</param>
        public void DownloadFilePartially(string address, string filepath, int timeout = 300)
        {
            DownloadFilePartially(new Uri(address), filepath, timeout);
        }

        /// <summary>
        ///     Загрузка файла порционно.
        /// </summary>
        /// <param name="address">Адрес (Конечная точка)</param>
        /// <param name="filepath">Путь созранения файла</param>
        /// <param name="timeout">Таймаут подключения в секундах</param>
        public void DownloadFilePartially(Uri address, string filepath, int timeout = 300)
        {
            DownloadFilePartiallyInternal(address, filepath, timeout);
        }

        /// <summary>
        ///     Загрузка файла порционно.
        /// </summary>
        /// <param name="address">Адрес (Конечная точка)</param>
        /// <param name="filepath">Путь созранения файла</param>
        /// <param name="timeout">Таймаут подключения в секундах</param>
        private void DownloadFilePartiallyInternal(Uri address, string filepath, int timeout = 300)
        {
            int TryBytesRead(byte[] buffer, int bytesRead, Stream responseStream, int tryingNum = 5)
            {
                try
                {
                    bytesRead = responseStream.Read(buffer, 0, 4096);
                    return bytesRead;
                }
                catch (Exception)
                {
                    tryingNum--;
                    if (tryingNum == 0)
                        throw;

                    Thread.Sleep(1000);
                    return TryBytesRead(buffer, bytesRead, responseStream, tryingNum);
                }
            }

            DateTime startTime = DateTime.UtcNow;

            WebRequest request = WebRequest.Create(address);
            request.Timeout = timeout;

            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                using (Stream fileStream = File.OpenWrite(filepath))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = responseStream.Read(buffer, 0, 4096);

                    var length = response.ContentLength;
                    var bytesReadPacks = 0;
                    while (bytesRead > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        bytesRead = TryBytesRead(buffer, bytesRead, responseStream);

                        bytesReadPacks++;

                        DateTime nowTime = DateTime.UtcNow;
                        if ((int)(nowTime - startTime).TotalSeconds % 60 == 0)
                        {
                            var totalBytes = bytesReadPacks * bytesRead;
                            var percentage = (bytesReadPacks * bytesRead) / length;
                            DownloadProgressPartiallyChanged?.Invoke(this,
                                new DownloadProgressChangedEventArgs(percentage, bytesRead, totalBytes));
                        }

                        if ((nowTime - startTime).TotalSeconds > timeout)
                        {
                            throw new ApplicationException(
                                "Download timed out");
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Загрузка файла порционно.
        /// </summary>
        /// <param name="address">Адрес (Конечная точка)</param>
        /// <param name="filepath">Путь созранения файла</param>
        /// <param name="timeout">Таймаут подключения в секундах</param>
        public async Task DownloadFilePartiallyAsync(string address, string filepath, int timeout = 300)
        {
            await DownloadFilePartiallyAsync(new Uri(address), filepath, timeout);
        }

        /// <summary>
        ///     Загрузка файла порционно.
        /// </summary>
        /// <param name="address">Адрес (Конечная точка)</param>
        /// <param name="filepath">Путь созранения файла</param>
        /// <param name="timeout">Таймаут подключения в секундах</param>
        public async Task DownloadFilePartiallyAsync(Uri address, string filepath, int timeout = 300)
        {
            await DownloadFilePartiallyInternalAsync(address, filepath, timeout);
        }

        /// <summary>
        ///     Загрузка файла порционно.
        /// </summary>
        /// <param name="address">Адрес (Конечная точка)</param>
        /// <param name="filepath">Путь созранения файла</param>
        /// <param name="timeout">Таймаут подключения в секундах</param>
        private async Task DownloadFilePartiallyInternalAsync(Uri address, string filepath, int timeout = 300)
        {
            async Task<int> TryBytesRead(byte[] buffer, int bytesRead, Stream responseStream, int tryingNum = 5)
            {
                try
                {
                    bytesRead = await responseStream.ReadAsync(buffer, 0, 4096);
                    return bytesRead;
                }
                catch (Exception)
                {
                    tryingNum--;
                    if (tryingNum == 0)
                        throw;

                    Thread.Sleep(1000);
                    return await TryBytesRead(buffer, bytesRead, responseStream, tryingNum);
                }
            }

            DateTime startTime = DateTime.UtcNow;

            WebRequest request = WebRequest.Create(address);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                using (Stream fileStream = File.OpenWrite(filepath))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = responseStream.Read(buffer, 0, 4096);

                    var length = response.ContentLength;
                    var bytesReadPacks = 0;
                    while (bytesRead > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        bytesRead = await TryBytesRead(buffer, bytesRead, responseStream);

                        bytesReadPacks++;

                        DateTime nowTime = DateTime.UtcNow;
                        if ((int)(nowTime - startTime).TotalSeconds % 60 == 0)
                        {
                            var totalBytes = bytesReadPacks * bytesRead;
                            var percentage = (bytesReadPacks * bytesRead) / length;
                            DownloadProgressPartiallyChanged?.Invoke(this,
                                new DownloadProgressChangedEventArgs(percentage, bytesRead, totalBytes));
                        }

                        if ((nowTime - startTime).TotalSeconds > timeout)
                        {
                            throw new ApplicationException(
                                "Download timed out");
                        }
                    }
                }
            }
        }
    }

    public class DownloadProgressChangedEventArgs : EventArgs
    {
        private readonly double _progressPercentage;
        private readonly long _bytesReceived;
        private readonly long _totalBytesToReceive;

        public DownloadProgressChangedEventArgs(double progressPercentage, long bytesReceived, long totalBytesToReceive)
        {
            _progressPercentage = progressPercentage;
            _bytesReceived = bytesReceived;
            _totalBytesToReceive = totalBytesToReceive;
        }

        public double ProgressPercentage => _progressPercentage;

        /// <summary>
        ///     Gets the number of bytes received.</summary>
        /// <returns>
        /// An <see cref="T:System.Int64" /> value that indicates the number of bytes received.</returns>
        public long BytesReceived => _bytesReceived;

        /// <summary>
        ///     Gets the total number of bytes in a <see cref="T:AddrSysWebClient" /> data download operation.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Int64" /> value that indicates the number of bytes that will be received.
        /// </returns>
        public long TotalBytesToReceive => _totalBytesToReceive;
    }

    public delegate void DownloadProgressChangedEventHandler(
        object sender,
        DownloadProgressChangedEventArgs e);
}