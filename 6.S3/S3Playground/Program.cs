using System.Net.Mime;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Model.Internal.MarshallTransformations;

const string bucketName = "f136aws-fundamentals";
var s3Client = new AmazonS3Client();

// Stream to img location
// await using var inputStream = new FileStream("./face.jpg", FileMode.Open, FileAccess.Read);
//
// var putObjectRequest = new PutObjectRequest();      
//
// putObjectRequest.BucketName = bucketName;
// putObjectRequest.Key = "images/face.jpeg";
// putObjectRequest.ContentType = "image/jpeg";
// putObjectRequest.InputStream = inputStream;
//
// await s3Client.PutObjectAsync(putObjectRequest); 
//
// await using var inputStream = new FileStream("./movies.csv", FileMode.Open, FileAccess.Read);
//
// var putObjectRequest = new PutObjectRequest();      
//
// putObjectRequest.BucketName = bucketName;
// putObjectRequest.Key = "files/movies.csv";
// putObjectRequest.ContentType = "text/csv";
// putObjectRequest.InputStream = inputStream;
//
// await s3Client.PutObjectAsync(putObjectRequest); 
//
//
// var getObjectRequest = new GetObjectRequest();
//
// getObjectRequest.BucketName = bucketName;
// getObjectRequest.Key = "files/movies.csv";
//
// var response = await s3Client.GetObjectAsync(getObjectRequest);
//
// using var memoryStream = new MemoryStream();
// response.ResponseStream.CopyTo(memoryStream);
//
// var text = Encoding.Default.GetString(memoryStream.ToArray());
//
// Console.WriteLine(text);

