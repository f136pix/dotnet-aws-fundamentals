using Amazon.SQS.Model;

namespace Customers.Api.Messaging;

public interface ISqsMessager
{
    Task<SendMessageResponse> SendMessageAsync<T>(T message);

}