using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReportMicroservice.Data
{
    public sealed class LogsRecord
    {
         [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string EventType { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
