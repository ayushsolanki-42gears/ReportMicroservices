namespace ReportMicroservice.Reports
{
    public interface IReportGenerator
    {
        string OutputExtension { get; }
        string OutputContentType { get; }
        Task<Stream> GenerateAsync<T>(IEnumerable<T> data, CancellationToken ct);
    }
}
