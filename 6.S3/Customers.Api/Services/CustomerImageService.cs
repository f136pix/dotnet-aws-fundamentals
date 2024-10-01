using System.Runtime.Intrinsics.X86;
using Amazon.S3;
using Amazon.S3.Model;

namespace Customers.Api.Services;

public class CustomerImageService : ICustomerImageService
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucketName = "f136aws-fundamentals";
    private readonly string awsPath = "usersImages";

    public CustomerImageService(IAmazonS3 s3)
    {
        _s3 = s3;
    }

    public async Task<PutObjectResponse> UploadImageAsync(Guid id, IFormFile file)
    {
        var putOjectRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = $"{awsPath}/{id}",
            ContentType = file.ContentType,
            InputStream = file.OpenReadStream(),
            Metadata =
            {
                ["x-amz-meta-originalname"] = file.FileName,
                ["x-amz-meta-extension"] = Path.GetExtension(file.FileName)
            }
        };

        return await _s3.PutObjectAsync(putOjectRequest);
    }

    public async Task<GetObjectResponse> GetImageAsync(Guid id)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = _bucketName, 
            Key = $"{awsPath}/{id}",
        };

        return await _s3.GetObjectAsync(getObjectRequest);
    }

    public async Task<DeleteObjectResponse> DeleteImageAsync(Guid id)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = $"{awsPath}/{id}"
        };

        return await _s3.DeleteObjectAsync(deleteObjectRequest);
    }
}