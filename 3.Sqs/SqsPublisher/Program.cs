using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Contract;
using SqsPublisher;

var sqsClient = new AmazonSQSClient();
var getQueueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var customer = new CustomerCreated()
{
    Id = Guid.NewGuid(),
    FullName = "John Doe",
    Email = "John@gmail.com",
    GithubUsername = "JohnDoe",
    DateOfBirth = new DateTime(2004, 6, 20)
};

var sendMessageRequest = new SendMessageRequest
{
    QueueUrl = getQueueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customer),
    MessageAttributes = new CustomerCreatedAttributes()
};

var response = await sqsClient.SendMessageAsync(sendMessageRequest);

Console.WriteLine(response.MessageId);
Console.WriteLine(response.HttpStatusCode);


