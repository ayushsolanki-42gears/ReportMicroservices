namespace ReportMicroservice.Storage;
public interface IS3Storage
{
    Task UploadAsync(string key, Stream content, string contentType, CancellationToken ct);
}
