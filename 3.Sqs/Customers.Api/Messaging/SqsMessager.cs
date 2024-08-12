using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using ThirdParty.Json.LitJson;

namespace Customers.Api.Messaging;

public class SqsMessager : ISqsMessager
{
    private readonly IAmazonSQS _sqsClient;
    private readonly IOptions<QueueSettings> _queueSettings;
    private string? _queueUrl;

    public SqsMessager(IAmazonSQS sqsClient, IOptions<QueueSettings> queueSettings)
    {
        _sqsClient = sqsClient;
        _queueSettings = queueSettings;
    }

    public async Task<SendMessageResponse> SendMessageAsync<T>(T message)
    {
        // Sets queue url if not cached
        await SetQueueUrlAsync();

        var sendMessage = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = GenerateMessageAttributes(typeof(T).Name)
        };

        return await _sqsClient.SendMessageAsync(sendMessage);
    }


    private Dictionary<string, MessageAttributeValue> GenerateMessageAttributes(string messageType)
    {
        return new Dictionary<string, MessageAttributeValue>
        {
            {
                "MessageType", new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = messageType
                }
            }
        };
    }

    private async Task SetQueueUrlAsync()
    {
        if (_queueUrl is not null)
        {
            return;
        }

        var result = await _sqsClient.GetQueueUrlAsync(_queueSettings.Value.Name);

        _queueUrl = result.QueueUrl;
    }
}