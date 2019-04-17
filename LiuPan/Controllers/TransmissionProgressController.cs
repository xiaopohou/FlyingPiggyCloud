using FileDownloader;
using SixCloud.Models;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net;

namespace SixCloud.Controllers
{
    internal class TransmissionProgressController
    {
        private static readonly string StoragePath = KnownFolders.Documents.Path + @"\SixCloud";

        private static readonly SqLiteHelper Helper;

        private const string DataSource = "SixCloud.db3";

        static TransmissionProgressController()
        {
            Directory.CreateDirectory(StoragePath);
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
            {
                Version = 3,
                DataSource = $"{StoragePath}\\{DataSource}",
                FailIfMissing = false,
                Pooling = true
            };
            Helper = new SqLiteHelper(builder.ConnectionString);
            DownloadingCache.StartUpRecovery();
        }

        internal class DownloadingCache : IDownloadCache
        {

            public DownloadingCache()
            {
                Helper.CreateTable("DownloadCache", new string[] { "uri", "path" }, new string[] { "TEXT", "TEXT" });
                Helper.CreateTable("DownloadTasksRecord", new string[] { "downloadAddress", "localPath" }, new string[] { "TEXT", "TEXT" });
            }

            //private List<DownloadTask> DownloadTasksRecord = new List<DownloadTask>();

            public static void AddRecord(DownloadTask task)
            {
                using (var reader = Helper.ReadFullTable("DownloadTasksRecord"))
                {
                    while(reader.Read())
                    {
                        if(reader.GetString(reader.GetOrdinal("downloadAddress"))==task.DownloadAddress&& reader.GetString(reader.GetOrdinal("localPath")) == task.Path)
                        {
                            return;
                        }
                    }
                }
                Helper.InsertValues("DownloadTasksRecord", new string[] { task.DownloadAddress, task.Path }).Close();
                task.DownloadFileCompleted += (sender, e) =>
                {
                    Helper.DeleteValuesOR("DownloadTasksRecord", new string[] { "downloadAddress", "localPath" }, new string[] { task.DownloadAddress, task.Path }, new string[] { "=", "=" }).Close();
                };
            }

            public static void StartUpRecovery()
            {
                using (var reader = Helper.ReadFullTable("DownloadTasksRecord"))
                {
                    while (reader.Read())
                    {
                        ViewModels.DownloadingListViewModel.NewTask(reader.GetString(reader.GetOrdinal("downloadAddress")), reader.GetString(reader.GetOrdinal("localPath")));
                    }
                }
            }

            public void Add(Uri uri, string path, WebHeaderCollection headers)
            {
                using (SQLiteDataReader reader = Helper.ReadFullTable("DownloadCache"))
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(reader.GetOrdinal("uri")) == uri.ToString())
                        {
                            Helper.UpdateValues("DownloadCache", new string[] { "uri" }, new string[] { uri.ToString() }, "path", path);
                            return;
                        }
                    }
                }
                Helper.InsertValues("DownloadCache", new string[] { uri.ToString(), path });
            }

            public string Get(Uri uri, WebHeaderCollection headers)
            {
                using (SQLiteDataReader reader = Helper.ReadFullTable("DownloadCache"))
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(reader.GetOrdinal("uri")) == uri.ToString())
                        {
                            string s = reader.GetString(reader.GetOrdinal("path"));
                            return s;
                        }
                    }
                }
                return null;
            }

            public void Invalidate(Uri uri)
            {
                using (SQLiteDataReader reader = Helper.ReadFullTable("DownloadCache"))
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(reader.GetOrdinal("uri")) == uri.ToString())
                        {
                            Helper.DeleteValuesAND("DownloadCache", new string[] { "uri" }, new string[] { uri.ToString() }, new string[] { "=" });
                            return;
                        }
                    }
                }
            }

            //private class Cache
            //{
            //    public string Uri { get; set; }
            //    public string Path { get; set; }
            //}

            //private class CacheDBContext : DbContext
            //{
            //    public CacheDBContext(DbConnection dbConnection) : base(dbConnection, true)
            //    {

            //    }
            //    public DbSet<Cache> DownloadCaches { get; set; }
            //}
        }
    }
}

