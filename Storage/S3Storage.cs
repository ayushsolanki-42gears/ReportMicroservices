using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using ReportMicroservice.Storage;

namespace ReportService.Storage;

public sealed class S3Options
{
    public string Region { get; set; } = default!;
    public string BucketName { get; set; } = default!;
    public string? AccessKeyId { get; set; }
    public string? SecretAccessKey { get; set; }
}

public sealed class S3Storage : IS3Storage
{
    private readonly IAmazonS3 _s3;
    private readonly S3Options _opt;

    public S3Storage(IOptions<S3Options> options)
    {
        _opt = options.Value;

        // Let the default credential chain pick credentials (env, role, user-secrets via profile), 
        // or use explicit keys if provided.
        _s3 = string.IsNullOrWhiteSpace(_opt.AccessKeyId) || string.IsNullOrWhiteSpace(_opt.SecretAccessKey)
            ? new AmazonS3Client(RegionEndpoint.GetBySystemName(_opt.Region))
            : new AmazonS3Client(_opt.AccessKeyId, _opt.SecretAccessKey, RegionEndpoint.GetBySystemName(_opt.Region));
    }

    public async Task UploadAsync(string key, Stream content, string contentType, CancellationToken ct)
    {
        var put = new PutObjectRequest
        {
            BucketName = _opt.BucketName,
            Key = key,
            InputStream = content,
            ContentType = contentType
        };
        await _s3.PutObjectAsync(put, ct);
    }
}
