namespace ReportMicroservice.Models
{
    public class ReportRequest
    {
        public string ReportId { get; init; } = default!;   // correlation id
        public string ReportType { get; init; } = "Logs";  // whatever you support
        public DateTime From { get; init; }
        public DateTime To { get; init; }
        public int UserId { get; init; }         // optional
        public List<string> Events { get; init; } = new();
    }
}