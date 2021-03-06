﻿using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Data.Exporter.Extensions
{
    public static class NpgsqlCommandExtensions
    {
        public static async Task<int> ExecuteFileAsync(this NpgsqlCommand cmd, string filename, Encoding enc = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var strText = File.ReadAllText(filename, enc ?? Encoding.UTF8);
            cmd.CommandText = strText;
            return await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}