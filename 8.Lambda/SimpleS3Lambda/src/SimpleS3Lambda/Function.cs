using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SimpleS3Lambda;

public class Function
{
    IAmazonS3 S3Client { get; set; }

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        S3Client = new AmazonS3Client();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client">The service client to access Amazon S3.</param>
    public Function(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// </summary>
    /// <param name="evnt">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
    {
                            context.Logger.LogInformation(
                                $"--> Entering Lambda");
        
        var eventRecords = evnt.Records ?? new List<S3Event.S3EventNotificationRecord>();
        foreach (var record in eventRecords)
        {
            var s3Event = record.S3;
            if (s3Event == null)
            {
                continue;
            }

            try
            {
                var response = await S3Client.GetObjectMetadataAsync(s3Event.Bucket.Name, s3Event.Object.Key);

                if (response.Metadata["x-amz-resized"] == true.ToString())
                {
                    context.Logger.LogInformation(
                        $"Item with key {record.S3.Object.Key} its a resized item, skipping it.");
                    continue;
                }

                Stream? itemStream = await S3Client.GetObjectStreamAsync(s3Event.Bucket.Name, s3Event.Object.Key, null);

                using var outStream = new MemoryStream();
                using (var image = await Image.LoadAsync(itemStream))
                {
                    image.Mutate(x => x.Resize(500, 500, KnownResamplers.Lanczos3));
                    var originalName = response.Metadata["x-amz-meta-originalname"] ?? "image.png";
                    await image.SaveAsync(outStream, image.DetectEncoder(originalName));
                }

                var request = new PutObjectRequest
                {
                    BucketName = s3Event.Bucket.Name,
                    Key = s3Event.Object.Key,
                    InputStream = outStream,
                    ContentType = response.Headers.ContentType,
                    Metadata =
                    {
                        ["x-amz-meta-originalname"] = response.Metadata["x-amz-meta-originalname"],
                        ["x-amz-meta-extension"] = response.Metadata["x-amz-meta-extension"],
                        ["x-amz-resized"] = true.ToString()
                    }
                };

                var res = await S3Client.PutObjectAsync(request);
                if (res.HttpStatusCode == HttpStatusCode.OK)
                {
                    context.Logger.LogInformation(
                        $"Successfully resized {s3Event.Object.Key} in bucket {s3Event.Bucket.Name}");
                }
                else
                {
                    context.Logger.LogError(
                        $"Error resizing {s3Event.Object.Key} in bucket {s3Event.Bucket.Name}");
                }

            }
            catch (Exception e)
            {
                context.Logger.LogError(
                    $"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                context.Logger.LogError(e.Message);
                context.Logger.LogError(e.StackTrace);
                throw;
            }
        }
    }
}