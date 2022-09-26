using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Universe.Helpers.Extensions;

namespace Universe.Fias.IO.Compression
{
    /// <summary>
    /// 7Zip archivator.
    /// </summary>
    public class Archive7Zip
    {
        /// <summary>
        /// Overwrite modes.
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum OverwriteModes
        {
            /// <summary>
            ///     -aoa Overwrite All existing files without prompt
            /// </summary>
            a,

            /// <summary>
            ///     -aos Skip extracting of existing files
            /// </summary>
            s,

            /// <summary>
            ///     -aou aUto rename extracting file (for example, name.txt will be renamed to name_1.txt)
            /// </summary>
            u,

            /// <summary>
            ///     -aot auto rename existing file (for example, name.txt will be renamed to name_1.txt).
            /// </summary>
            t
        }

        private readonly string _sevenZipFolderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="Archive7Zip"/> class.
        /// </summary>
        /// <param name="sevenZipFolderPath">The seven zip folder path.</param>
        /// <exception cref="System.ArgumentNullException">sevenZipFolderPath</exception>
        public Archive7Zip([NotNull] string sevenZipFolderPath)
        {
            if (sevenZipFolderPath == null)
            {
                throw new ArgumentNullException(nameof(sevenZipFolderPath));
            }

            _sevenZipFolderPath = sevenZipFolderPath;
        }

        /// <summary>
        /// Lists the contents.
        /// </summary>
        /// <param name="archiveFilePath">The archive file path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">archiveFilePath</exception>
        /// <exception cref="System.NotSupportedException">
        /// </exception>
        /// <exception cref="System.Exception"></exception>
        public List<string> ListContents([NotNull] string archiveFilePath)
        {
            if (archiveFilePath.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(archiveFilePath));
            }

            try
            {
                var str = StartProcess7Z($@"l ""{archiveFilePath}""");

                /*
                   Date      Time    Attr         Size   Compressed  Name
                ------------------- ----- ------------ ------------  ------------------------
                2009-10-08 13:12:00 .R..A         1466       450845  Asm\arm\7zCrcOpt.asm
                2012-12-30 12:41:54 .R..A         1499               Asm\x86\7zAsm.asm
                2009-12-12 18:10:08 .R..A         2817               Asm\x86\7zCrcOpt.asm
                2009-12-12 18:10:02 .R..A         3991               Asm\x86\AesOpt.asm
                2011-06-28 17:41:03 .R..A         3686               Asm\x86\XzCrc64Opt.asm
                2017-04-03 16:12:17 .R..A         5275               C\7z.h
                2017-04-03 14:58:28 .R..A         1595               C\7zAlloc.c
                2017-04-03 14:58:28 .R..A          385               C\7zAlloc.h
                2017-04-29 12:01:16 ....A       747520               bin\x64\7zr.exe
                ------------------- ----- ------------ ------------  ------------------------
                2017-04-29 13:03:17            4609678       973096  626 files
                */


                var lines = str.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                var linesQueue = new Queue<string>(lines);
                string line = null;
                while (linesQueue.Count > 0)
                {
                    line = linesQueue.Dequeue();
                    if (line.Contains("Date") && line.Contains("Time") && line.Contains("Attr")
                        && line.Contains("Size") && line.Contains("Compressed") && line.Contains("Name"))
                    {
                        break;
                    }
                }

                var notSupportedExceptionMessage = $@"Not supported response of list contents of archive ""{archiveFilePath}""
StandardOutput:
{str}";
                if (linesQueue.Count == 0)
                {
                    throw new NotSupportedException(notSupportedExceptionMessage);
                }

                line = linesQueue.Dequeue();
                if (!(line.StartsWith("-----") && line.EndsWith("-----")))
                {
                    throw new NotSupportedException(notSupportedExceptionMessage);
                }

                var fileNames = new List<string>();

                while (linesQueue.Count > 0)
                {
                    line = linesQueue.Dequeue();
                    if (line.StartsWith("-----"))
                    {
                        break;
                    }

                    var arr = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var fileName = arr.LastOrDefault();
                    if (!fileName.IsNullOrEmpty())
                    {
                        fileNames.Add(fileName);
                    }
                }

                return fileNames;
            }
            catch (Exception ex)
            {
                throw new Exception($@"List contents of archive ""{archiveFilePath}"" error.", ex);
            }
        }

        /// <summary>
        /// Unpacks the archive.
        /// </summary>
        /// <param name="archiveFilePath">The archive file path.</param>
        /// <param name="outPutFolderPath">The out put folder path.</param>
        /// <param name="overwriteMode">The overwrite mode.</param>
        /// <param name="assumeYesOnAllQueries">if set to <c>true</c> [assume yes on all queries].</param>
        /// <param name="wildcardOrFileNameList">The wildcard or file name list.</param>
        /// <exception cref="System.ArgumentNullException">
        /// archiveFilePath
        /// or
        /// outPutFolderPath
        /// </exception>
        /// <exception cref="System.Exception"></exception>
        public void UnpackArchive([NotNull] string archiveFilePath,
            [NotNull] string outPutFolderPath,
            OverwriteModes? overwriteMode = null,
            bool assumeYesOnAllQueries = false,
            IEnumerable<string> wildcardOrFileNameList = null)
        {
            if (archiveFilePath.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(archiveFilePath));
            }
            if (outPutFolderPath.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(outPutFolderPath));
            }

            var argsSb = new StringBuilder($@"x ""{archiveFilePath}"" -o""{outPutFolderPath}""");

            if (overwriteMode != null)
            {
                argsSb.Append($" -ao{overwriteMode}");
                // Overwrite mode -ao[a | s | t | u ] https://sevenzip.osdn.jp/chm/cmdline/switches/overwrite.htm
            }

            if (wildcardOrFileNameList != null)
            {
                // https://sevenzip.osdn.jp/chm/cmdline/switches/include.htm
                foreach (var wildcardOrFileName in wildcardOrFileNameList)
                {
                    argsSb.Append($@" -i!""{wildcardOrFileName}""");
                }
            }

            if (assumeYesOnAllQueries)
            {
                argsSb.Append(" -y"); // assume Yes on all queries
            }

            try
            {
                StartProcess7Z(argsSb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($@"Unpack archive ""{archiveFilePath}"" to ""{outPutFolderPath}"" error.", ex);
            }
        }

        private string StartProcess7Z(string arguments)
        {
            //"C:\Program Files\7-Zip\7z.exe" x "E:\Distr\lzma1700.7z" -o"c:\Doc2 3"
            var pInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(_sevenZipFolderPath, "7z.exe"),
                Arguments = arguments,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };

            using (var process = new Process
            {
                StartInfo = pInfo
            })
            {
                try
                {
                    process.Start();
                    using (var standardOutputReader = new ProcessOutputAsyncReader(
                        process,
                        process.StandardOutput.BaseStream,
                        process.StandardOutput.CurrentEncoding))
                    {
                        using (var standardErrorReader = new ProcessOutputAsyncReader(
                            process,
                            process.StandardError.BaseStream,
                            process.StandardError.CurrentEncoding))
                        {
                            standardOutputReader.Readed += (sender, args) =>
                            {
                                var outStr = standardOutputReader.GetOutputString();
                                if (outStr?.Contains("(Q)uit?") == true)
                                {
                                    standardOutputReader.StandardInputWriteLine("q");
                                    process.StandardInput.WriteLine("q");
                                }
                            };

                            standardOutputReader.BeginRead();
                            standardErrorReader.BeginRead();

                            process.WaitForExit();

                            standardOutputReader.WaitForAllRead();
                            standardErrorReader.WaitForAllRead();

                            if (process.ExitCode != 0)
                            {
                                throw new Exception($@"ExitCode={process.ExitCode}
Process: {pInfo.FileName} {pInfo.Arguments}

StandardOutput:
{standardOutputReader.GetOutputString()}

StandardError:
{standardErrorReader.GetOutputString()}");
                            }

                            return standardOutputReader.GetOutputString();
                        }
                    }
                }
                catch //(ThreadAbortException)
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }

                    throw;
                }
            }
        }
    }
}