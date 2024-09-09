using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SnsPublisher;

var customer = new CustomerCreated
{
     Id = Guid.NewGuid(),
     FullName = "Filipe Cinel",
     Email = "Filipecocinel@hotmail.com",
     DateOfBirth = new DateTime(2004, 6, 20),
     GitHubUsername = "f136pix"
};

var snsClient = new AmazonSimpleNotificationServiceClient();

var topicArn = snsClient.FindTopicAsync("customers").Result.TopicArn;

 var publishRequest = new PublishRequest
 {
     TopicArn = topicArn,
     Message = JsonSerializer.Serialize(customer),
     MessageAttributes = new Dictionary<string, MessageAttributeValue>
     {
         {
             "MessageType", new MessageAttributeValue
             {
                 DataType = "String",
                 StringValue = nameof(CustomerCreated)
             }
         }
     }
 };

 var response = await snsClient.PublishAsync(publishRequest);
 Console.WriteLine(response.HttpStatusCode);



