namespace ReportMicroservice.Messaging
{
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; } = default!;
        public string? GroupId { get; set; }
        public string? RequestTopic { get; set; }
        public string? ResponseTopic { get; set; }
    }
}