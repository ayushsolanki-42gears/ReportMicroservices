using ReportMicroservice.Models;

namespace ReportMicroservice.Messaging
{
    public interface IReportEventProducer
    {
        Task PublishReportCompletedAsync(ReportResponse evt, CancellationToken ct);
    }
}
