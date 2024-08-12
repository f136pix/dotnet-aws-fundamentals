using Amazon.SQS;
using Amazon.SQS.Model;
using Contract;

namespace SqsPublisher;

public class CustomerCreatedAttributes : Dictionary<string, MessageAttributeValue>
{
    public CustomerCreatedAttributes()
    {
        this["MessageType"] = new MessageAttributeValue
        {
            DataType = "String",
            StringValue = nameof(CustomerCreated)
        };
    }
}