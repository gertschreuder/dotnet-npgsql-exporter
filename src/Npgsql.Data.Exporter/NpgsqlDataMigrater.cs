using Npgsql.Data.Exporter.Extensions;
using Npgsql.Data.Exporter.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Data.Exporter
{
    public class NpgsqlDataMigrater : DataExporter
    {
        public NpgsqlDataMigrater(NpgsqlDataExporterConfiguration config) : base(config)
        {
        }

        public override async Task<string> Execute(CancellationToken cancellationToken = default(CancellationToken), string fileDirectory = null)
        {
            SetFilePath(fileDirectory);

            await using (var con = new NpgsqlConnection(ConnectionString))
            {
                await con.OpenAsync(cancellationToken).ConfigureAwait(false);

                using (var cmd = new NpgsqlCommand("", con))
                {
                    string[] files = System.IO.Directory.GetFiles(FileDirectory, "*.sql");
                    foreach (var file in files)
                    {
                        var result = await cmd.ExecuteFileAsync(filename: file, cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                }

                await con.CloseAsync().ConfigureAwait(false);
            }

            return FileDirectory;
        }
    }
}