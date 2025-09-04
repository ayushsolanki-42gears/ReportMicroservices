using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportMicroservice.Data;
using ReportMicroservice.Messaging;
using ReportMicroservice.Models;
using ReportMicroservice.Reports;
using ReportMicroservice.Storage;

namespace ReportMicroservice.Workers
{
    public class ReportWorker : BackgroundService
    {
        private readonly IReportRequestConsumer _consumer;
        private readonly IReportEventProducer _producer;
        private readonly IMongoRepository _repository;
        private readonly IReportGenerator _generator;
        private readonly IS3Storage _storage;
        private readonly ILogger<ReportWorker> _logger;

        public ReportWorker(
            IReportRequestConsumer consumer,
            IReportEventProducer producer,
            IMongoRepository repository,
            IReportGenerator generator,
            IS3Storage storage,
            ILogger<ReportWorker> logger
        )
        {
            _consumer = consumer;
            _producer = producer;
            _repository = repository;
            _generator = generator;
            _storage = storage;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReportWorker started");

            await _consumer.ConsumeAsync(HandleReportRequestAsync, stoppingToken);
            _logger.LogInformation("ReportWroker stopped");
        }

        private async Task HandleReportRequestAsync(ReportRequest request)
        {
            _logger.LogInformation("Processing ReportRequest {ReportId}", request.ReportId);
            
            try
            {
                var data = await _repository.GetLogsRecordAsync(request.From, request.To, request.Events);
                _logger.LogInformation("Fetch {Count} records from MongoDB", data.Count);

                using var reportStream = await _generator.GenerateAsync(data, CancellationToken.None);

                var s3Key = $"reports/{request.ReportId}{_generator.OutputExtension}";
                await _storage.UploadAsync(s3Key, reportStream, _generator.OutputContentType, CancellationToken.None);
                _logger.LogInformation("Report uploaded to S3 with key {Key}", s3Key);

                var completed = new ReportResponse
                {
                    ReportId = request.ReportId,
                    S3Key = s3Key,
                    Status = "Completed",
                    GeneratedAt = DateTime.UtcNow
                };

                await _producer.PublishReportCompletedAsync(completed, CancellationToken.None);
                _logger.LogInformation("ReportCompleted published for {ReportId}", request.ReportId);
            }
            catch (Exception ex)
            {
_logger.LogError(ex, "Error processing ReportRequest {ReportId}", request.ReportId);

                // Publish failed event
                var failed = new ReportResponse
                {
                    ReportId = request.ReportId,
                    Status = "Failed",
                    GeneratedAt = DateTime.UtcNow,
                    ErrorMessage = ex.Message
                };

                await _producer.PublishReportCompletedAsync(failed, CancellationToken.None);
            }
        }
        
    }
}