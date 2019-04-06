//----------------------------------------------------------------------------------------------------
// <copyright company="Avira Operations GmbH & Co. KG and its licensors">
// © 2016 Avira Operations GmbH & Co. KG and its licensors.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using FileDownloader.Logging;

namespace FileDownloader
{
    internal static class FileHelpers
    {
        private static ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();

        private static readonly ILogger Logger = LoggerFacade.GetCurrentClassLogger();

        public static bool TryGetFileSize(string filename, out long filesize)
        {
            try
            {
                var fileInfo = new FileInfo(filename);
                filesize = fileInfo.Length;
            }
            catch (Exception e)
            {
                Logger.Debug("Failed to get file size for {0}. Exception: {1}", filename, e.Message);
                filesize = 0;
                return false;
            }
            return true;
        }

        public static bool TryFileDelete(string filename)
        {
            try
            {
                readerWriterLockSlim.EnterWriteLock();
                File.Delete(filename);
            }
            catch (Exception e)
            {
                Logger.Debug("Unable to delete file {0}. Exception: {1}", filename, e.Message);
                return false;
            }
            finally
            {
                readerWriterLockSlim.ExitWriteLock();
            }
            return true;
        }

        public static bool ReplaceFile(string source, string destination)
        {
            if (!destination.Equals(source, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    readerWriterLockSlim.EnterWriteLock();
                    File.Delete(destination);
                    File.Move(source, destination);
                }
                catch (Exception e)
                {
                    Logger.Warn("Unable replace local file {0} with cached resource {1}, {2}", destination, source, e.Message);
                    return false;
                }
                finally
                {
                    readerWriterLockSlim.ExitWriteLock();
                }
            }
            return true;
        }
    }
}