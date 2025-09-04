namespace ReportMicroservice.Data
{
    public interface IMongoRepository
    {
        Task<List<LogsRecord>> GetLogsRecordAsync(
            DateTime? from,
            DateTime? to,
            List<string>? events,
            CancellationToken ct = default
        );
    }
}