using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Data.Exporter.Models;

namespace Npgsql.Data.Exporter
{
    public abstract class DataExporter: IDataExporter
    {
        protected readonly string ConnectionString;
        protected string FileDirectory;

        public DataExporter(NpgsqlDataExporterConfiguration config)
        {
            ConnectionString = config.SourceConnectionString ?? throw new ArgumentNullException(nameof(config.SourceConnectionString));
            FileDirectory = config.FileDirectory ?? Directory.GetCurrentDirectory();
        }
        public abstract Task<string> Execute(CancellationToken cancellationToken = default(CancellationToken), string fileDirectory = null);

        protected void SetFilePath(string fileDirectory)
        {
            FileDirectory = fileDirectory ?? FileDirectory.Trim();
            if (!string.IsNullOrWhiteSpace(FileDirectory))
            {
                if (!Directory.Exists(FileDirectory))
                {
                    Directory.CreateDirectory(FileDirectory);
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    if (!FileDirectory.EndsWith("/"))
                        FileDirectory += "/";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (!FileDirectory.EndsWith("\\"))
                        FileDirectory += "\\";
                }
            }
        }
    }
}