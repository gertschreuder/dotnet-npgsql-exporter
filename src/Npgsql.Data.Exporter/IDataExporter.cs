using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Data.Exporter
{
    public interface IDataExporter
    {
        Task<string> Execute(CancellationToken cancellationToken = default(CancellationToken),
            string fileDirectory = null);
    }
}