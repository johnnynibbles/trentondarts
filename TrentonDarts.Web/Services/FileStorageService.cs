using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Services;

public interface IFileStorageService
{
    Task UploadAsync(Stream content, string objectKey, string contentType);
    Task DeleteAsync(string objectKey);
    string GetPublicUrl(string objectKey);
}

public class S3FileStorageService : IFileStorageService, IDisposable
{
    private readonly AmazonS3Client _client;
    private readonly StorageOptions _options;

    public S3FileStorageService(IOptions<StorageOptions> options)
    {
        _options = options.Value;
        var config = new AmazonS3Config
        {
            ServiceURL = _options.ServiceUrl,
            ForcePathStyle = _options.ForcePathStyle,
            AuthenticationRegion = _options.Region
        };
        _client = new AmazonS3Client(
            new BasicAWSCredentials(_options.AccessKey, _options.SecretKey),
            config);
    }

    public async Task UploadAsync(Stream content, string objectKey, string contentType)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            InputStream = content,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };
        await _client.PutObjectAsync(request);
    }

    public async Task DeleteAsync(string objectKey)
    {
        await _client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey
        });
    }

    public string GetPublicUrl(string objectKey) =>
        $"{_options.PublicBaseUrl.TrimEnd('/')}/{objectKey}";

    public void Dispose() => _client.Dispose();
}
