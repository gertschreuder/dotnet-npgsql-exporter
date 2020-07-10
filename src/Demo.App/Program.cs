using Npgsql.Data.Exporter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var cts = new CancellationTokenSource();

                var exporter = new NpgsqlDataExporter();
                var fileLocation = await exporter.Execute(cts.Token)
                    .ConfigureAwait(false);

                var migrater = new NpgsqlDataMigrater();
                await migrater.Execute(cts.Token, fileLocation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}