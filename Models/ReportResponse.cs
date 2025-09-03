namespace ReportMicroservice.Models
{
    public sealed class ReportResponse
    {
        public string ReportId { get; init; } = default!;
        public string S3Key { get; init; } = default!;
        public string Status { get; init; } = "Completed";
        public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
        public string? ErrorMessage { get; init; }
    }
}