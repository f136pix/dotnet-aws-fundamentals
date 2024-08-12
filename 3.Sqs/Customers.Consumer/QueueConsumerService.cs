using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Options;

namespace Customers.Consumer;

public class QueueConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IOptions<QueueSettings> _queueSettings;
    private readonly ISender _mediator;

    public QueueConsumerService(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings, ISender mediator)
    {
        _sqs = sqs;
        _queueSettings = queueSettings;
        _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var getQueueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name, stoppingToken);

        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = getQueueUrlResponse.QueueUrl,
            MessageAttributeNames = new List<string> { "All " },
            MaxNumberOfMessages = 1
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
            foreach (var m in response.Messages)
            {
                var messageType = m.MessageAttributes["MessageType"].StringValue;
                Type? type = Type.GetType($"Customers.Consumer.Messages.{messageType}"!);
                if (type is null)
                {
                    Console.WriteLine($"Message type {messageType} not found");
                    continue;
                }

                ISqsMessage typedMessage = (ISqsMessage)JsonSerializer.Deserialize(m.Body, type)!;

                try
                {
                    await _mediator.Send(typedMessage, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error processing message: " + ex.Message);
                    continue;
                }
                
                await _sqs.DeleteMessageAsync(getQueueUrlResponse.QueueUrl, m.ReceiptHandle, stoppingToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}