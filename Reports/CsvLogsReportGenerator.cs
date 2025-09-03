using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using ReportMicroservice.Reports;
using System.Globalization;
using System.Text;

namespace ReportService.Reports
{
    public sealed class CsvSalesReportGenerator : IReportGenerator
    {
        private readonly ILogger<CsvSalesReportGenerator> _logger;

        public CsvSalesReportGenerator(ILogger<CsvSalesReportGenerator> logger)
        {
            _logger = logger;
        }

        public string OutputExtension => ".csv";
        public string OutputContentType => "text/csv";

        public async Task<Stream> GenerateAsync<T>(IEnumerable<T> data, CancellationToken ct)
        {
            _logger.LogInformation("Generating CSV report with {Count} records", data.Count());

            var memory = new MemoryStream();
            using var writer = new StreamWriter(memory, Encoding.UTF8, leaveOpen: true);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

            await csv.WriteRecordsAsync(data, ct);
            await writer.FlushAsync();

            memory.Position = 0; // reset for reading
            return memory;
        }
    }
}
