using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ReportMicroservice.Data
{
    public sealed class MongoRepository : IMongoRepository
    {
        private readonly IMongoCollection<LogsRecord> _collection;
        private readonly ILogger<MongoRepository> _logger;

        public MongoRepository(IOptions<MongoOptions> option, ILogger<MongoRepository> logger)
        {
            var opt = option.Value;
            _logger = logger;

            var client = new MongoClient(opt.ConnectionString);
            var database = client.GetDatabase(opt.DatabaseName);
            _collection = database.GetCollection<LogsRecord>(opt.CollectionName);
        }

        public async Task<List<LogsRecord>> GetLogsRecordAsync(
            DateTime? from,
            DateTime? to,
            List<string>? events,
            CancellationToken ct = default)
        {
            var builder = Builders<LogsRecord>.Filter;
            FilterDefinition<LogsRecord> filter = builder.Empty;


            if (from.HasValue)
            {
                filter &= builder.Gte(x => x.CreatedAt, from.Value);
            }

            if (to.HasValue)
            {
                filter &= builder.Lte(x => x.CreatedAt, to.Value);
            }

            Console.WriteLine(events.ToString());

            if (events != null && events.Count > 0)
            {
                filter &= builder.In(x => x.EventType, events);
            }

            _logger.LogInformation(
                "Querying MongoDB for logs: from={From}, to={To}, events={Events}",
                from?.ToString("u") ?? "ALL",
                to?.ToString("u") ?? "ALL",
                events == null || events.Count == 0 ? "ALL" : string.Join(",", events));

            return await _collection.Find(filter).ToListAsync(ct);
        }
    }
}