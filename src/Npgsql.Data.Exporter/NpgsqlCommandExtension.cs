using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace Npgsql.Data.Exporter
{
    public static class NpgsqlCommandExtension
    {
        public static async Task<int> ExecuteFileAsync(this NpgsqlCommand cmd, string filename, Encoding enc = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var strText = System.IO.File.ReadAllText(filename, enc ?? Encoding.UTF8);
            cmd.CommandText = strText;
            return await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}