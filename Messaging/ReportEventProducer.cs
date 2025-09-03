using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportMicroservice.Messaging;
using ReportMicroservice.Models;
using System.Text.Json;

namespace ReportService.Messaging
{
    public sealed class KafkaReportEventProducer : IReportEventProducer
    {
        private readonly KafkaOptions _options;
        private readonly ILogger<KafkaReportEventProducer> _logger;

        public KafkaReportEventProducer(
            IOptions<KafkaOptions> options,
            ILogger<KafkaReportEventProducer> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task PublishReportCompletedAsync(ReportResponse evt, CancellationToken ct)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _options.BootstrapServers
            };

            using var producer = new ProducerBuilder<string, string>(config).Build();

            var message = new Message<string, string>
            {
                Key = evt.ReportId,
                Value = JsonSerializer.Serialize(evt)
            };

            var result = await producer.ProduceAsync(_options.ResponseTopic, message, ct);

            _logger.LogInformation(
                "Published ReportCompleted event for ReportId={ReportId} to topic {Topic} at offset {Offset}",
                evt.ReportId, _options.ResponseTopic, result.Offset);
        }
    }
}
