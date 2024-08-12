using Amazon.SQS;
using Amazon.SQS.Model;

var cts = new CancellationTokenSource();

var sqsClient = new AmazonSQSClient();
var getQueueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var receiveMessageRequest = new ReceiveMessageRequest()
{
    QueueUrl = getQueueUrlResponse.QueueUrl,
    MaxNumberOfMessages = 1,
    WaitTimeSeconds = 20
};

while (!cts.IsCancellationRequested)
{
    var responseMessage = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cts.Token);

    foreach (var message in responseMessage.Messages)
    {
        Console.WriteLine($"--> MessageId : {message.MessageId}");
        Console.WriteLine($"--> Body : {message.Body}");
        
        // Ack, delete message
        await sqsClient.DeleteMessageAsync(getQueueUrlResponse.QueueUrl, message.ReceiptHandle);
    }

    await Task.Delay(1000);
}