using Npgsql.Data.Exporter;
using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Data.Exporter.Extensions;

namespace Demo.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var config = ConfigurationExtensions.GetNpgsqlDataExporter();
                var cts = new CancellationTokenSource();

                IDataExporter exporter = new NpgsqlDataExporter(config);
                var fileLocation = await exporter.Execute(cts.Token)
                    .ConfigureAwait(false);

                IDataExporter migrater = new NpgsqlDataMigrater(config);
                await migrater.Execute(cts.Token, fileLocation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}