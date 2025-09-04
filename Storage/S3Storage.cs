using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace ReportMicroservice.Storage;

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
    private readonly AwsOptions _opt;

    public S3Storage(IOptions<AwsOptions> options)
    {
        _opt = options.Value;

        Console.WriteLine("AccessKey : " + _opt.AccessKey);
        Console.WriteLine("Secret Key : " + _opt.SecretKey);

        // Let the default credential chain pick credentials (env, role, user-secrets via profile), 
        // or use explicit keys if provided.
        _s3 = string.IsNullOrWhiteSpace(_opt.AccessKey) || string.IsNullOrWhiteSpace(_opt.SecretKey)
            ? new AmazonS3Client(RegionEndpoint.GetBySystemName(_opt.Region))
            : new AmazonS3Client(_opt.AccessKey, _opt.SecretKey, RegionEndpoint.GetBySystemName(_opt.Region));
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
