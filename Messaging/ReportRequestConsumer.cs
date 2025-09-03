using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportMicroservice.Models;

namespace ReportMicroservice.Messaging
{
    public sealed class ReportRequestConsumer : IReportRequestConsumer
    {
        private readonly KafkaOptions _options;
        private readonly ILogger _logger;

        public ReportRequestConsumer(
            IOptions<KafkaOptions> options,
            ILogger logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task ConsumeAsync(Func<ReportRequest, Task> handleRequest, CancellationToken ct)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                GroupId = _options.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Null, string>(config).Build();
            consumer.Subscribe(_options.RequestTopic);

            _logger.LogInformation("Listening for report requests on topic {Topic}", _options.RequestTopic);

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(ct);
                        if (result?.Message?.Value != null)
                        {
                            var request = JsonSerializer.Deserialize<ReportRequest>(result.Message.Value);
                            if (request != null)
                            {
                                _logger.LogInformation("Received ReportRequest: {ReportId}", request.ReportId);
                                await handleRequest(request);
                            }
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Kafka consume error");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer cancelled.");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}