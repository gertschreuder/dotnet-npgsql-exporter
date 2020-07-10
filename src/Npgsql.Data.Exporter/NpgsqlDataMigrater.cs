using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace Npgsql.Data.Exporter
{
    public class NpgsqlDataMigrater
    {
        private string _connectionString = "Host=localhost;User ID=demo_user;Password=d3m0_pa55w0rd;Port=5432;Database=demo_database;";
        private string _fileLocation = Directory.GetCurrentDirectory();

        public async Task Execute(CancellationToken cancellationToken = default(CancellationToken), string fileLocation = null)
        {
            await using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync(cancellationToken).ConfigureAwait(false);

                using (var cmd = new NpgsqlCommand("", con))
                {
                    string[] files = System.IO.Directory.GetFiles(_fileLocation, "*.sql");
                    foreach (var file in files)
                    {
                        var result = await cmd.ExecuteFileAsync(filename: file, cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                }

                await con.CloseAsync().ConfigureAwait(false);
            }
        }
    }
}