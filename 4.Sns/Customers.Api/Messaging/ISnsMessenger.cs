using Amazon.SimpleNotificationService.Model;
using Amazon.SQS.Model;

namespace Customers.Api.Messaging;

public interface ISnsMessenger
{
    Task<PublishResponse> PublishMessageAsync<T>(T message);
}
