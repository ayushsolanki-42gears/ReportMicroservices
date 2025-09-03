using ReportMicroservice.Models;

namespace ReportMicroservice.Messaging
{
    public interface IReportRequestConsumer{
        Task ConsumeAsync(Func<ReportRequest, Task> handleRequest, CancellationToken ct);
    }
}