namespace Npgsql.Data.Exporter.Models
{
    public class NpgsqlDataExporterConfiguration
    {
        public string FileDirectory { get; set; }
        public string SourceConnectionString { get; set; }
        public string TargetConnectionString { get; set; }
    }
}