using Npgsql.Data.Exporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Data.Exporter
{
    public class NpgsqlDataExporter : DataExporter
    {
        public NpgsqlDataExporter(NpgsqlDataExporterConfiguration config):base(config)
        {
        }

        public override async Task<string> Execute(CancellationToken cancellationToken = default(CancellationToken), string fileDirectory = null)
        {
            SetFilePath(fileDirectory);

            await using (var con = new NpgsqlConnection(ConnectionString))
            {
                await con.OpenAsync(cancellationToken).ConfigureAwait(false);

                var sql =
                    @"SELECT schemaname , tablename FROM pg_catalog.pg_tables WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema' 
                        and schemaname !='public';";

                using (var cmd = new NpgsqlCommand(sql, con))

                {
                    var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        await WriteTableData(reader["schemaname"].ToString(), reader["tablename"].ToString(), cancellationToken).ConfigureAwait(false);
                    }

                    await con.CloseAsync().ConfigureAwait(false);
                }
            }

            return FileDirectory;
        }

        private async Task WriteTableData(string schema, string tableName, CancellationToken cancellationToken = default(CancellationToken))
        {
            await using (var con = new NpgsqlConnection(ConnectionString))
            {
                await con.OpenAsync(cancellationToken).ConfigureAwait(false);

                var sql = $"SELECT * FROM {schema}.{tableName};";
                 using (var cmd = new NpgsqlCommand(sql, con))
                {
                    var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                    if (reader.HasRows)
                    {
                        if (reader.FieldCount > 500)
                        {
                            await IterateReaderLarge(reader, schema, tableName, cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            await IterateReader(reader, schema, tableName, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }

                await con.CloseAsync().ConfigureAwait(false);
            }
        }

        private async Task IterateReaderLarge(NpgsqlDataReader reader, string schema, string tableName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var selector = new List<string>();
            var sb = new StringBuilder();
            var counter = 0;
            var saveIteration = 1;
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (counter % 500 == 0)
                {
                    sb = new StringBuilder();
                    sb.Length--;
                    var strI = $"INSERT INTO {schema}.{tableName} ({string.Join(",", selector)}) VALUES";
                    File.WriteAllText($"{FileDirectory}{schema}-{tableName}-{saveIteration}.sql", $"{strI} {sb.ToString()};");
                    saveIteration++;
                }
                sb.Append("(");
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var val = FormatValueFromDataType(reader.GetFieldType(i), reader[i]);
                    sb.Append($"{val},");
                    if (!selector.Count().Equals(reader.FieldCount))
                    {
                        selector.Add(reader.GetName(i));
                    }
                }
                sb.Length--;
                sb.Append("),");
                counter++;
            }

            sb.Length--;
            var str = $"INSERT INTO {schema}.{tableName} ({string.Join(",", selector)}) VALUES";
            File.WriteAllText($"{FileDirectory}{schema}-{tableName}-{saveIteration}.sql", $"{str} {sb.ToString()};");
        }

        private async Task IterateReader(NpgsqlDataReader reader, string schema, string tableName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var selector = new List<string>();
            var sb = new StringBuilder();
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                sb.Append("(");
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var val = FormatValueFromDataType(reader.GetFieldType(i), reader[i]);
                    sb.Append($"{val},");
                    if (!selector.Count().Equals(reader.FieldCount))
                    {
                        selector.Add(reader.GetName(i));
                    }
                }
                sb.Length--;
                sb.Append("),");
            }

            sb.Length--;
            var str = $"INSERT INTO {schema}.{tableName} ({string.Join(",", selector)}) VALUES";
            File.WriteAllText($"{FileDirectory}{schema}-{tableName}.sql", $"{str} {sb.ToString()};");
        }

        private object FormatValueFromDataType(Type type, object val)
        {
            switch (type)
            {
                case Type numType when numType == typeof(int) ||
                                       numType == typeof(long) ||
                                       numType == typeof(decimal) ||
                                       numType == typeof(float):
                    return val;
                case Type stringType when stringType == typeof(string) ||
                                          stringType == typeof(Guid):
                    return $"'{val}'";
                case Type dtType when dtType == typeof(DateTime):
                    return val;
                default:
                    return val;
            }
        }
    }
}