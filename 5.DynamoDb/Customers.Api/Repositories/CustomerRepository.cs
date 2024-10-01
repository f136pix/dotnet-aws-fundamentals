using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Documents;
using Customers.Api.Contracts.Data;
using Document = Amazon.DynamoDBv2.DocumentModel.Document;

namespace Customers.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName = "customers";

    public CustomerRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task<bool> CreateAsync(CustomerDto customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);

        // Serializing to DB document
        var customerAsAttribute = Document.FromJson(customerAsJson).ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = customerAsAttribute,
            ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)"
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<CustomerDto?> GetAsync(Guid id)
    {
        var getUserAttribute = new Dictionary<string, AttributeValue>
        {
            { "pk", new AttributeValue { S = id.ToString() } },
            { "sk", new AttributeValue { S = id.ToString() } }
        };

        var getItemRequest = new GetItemRequest(_tableName, getUserAttribute);

        var response = await _dynamoDb.GetItemAsync(getItemRequest);

        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsJson = (Document.FromAttributeMap(response.Item)).ToJson();
        var itemAsDto = JsonSerializer.Deserialize<CustomerDto>(itemAsJson);

        return itemAsDto;
    }

    public async Task<CustomerDto?> GetByEmailAsync(string email)
    {
        var queryRequest = new QueryRequest
        {
            TableName = _tableName,
            IndexName = "email-id-index",
            KeyConditionExpression = "Email = :v_Email",
            ExpressionAttributeValues =
            {
                {
                    "v_Email", new AttributeValue { S = email }
                }
            }
        };

        var response = await _dynamoDb.QueryAsync(queryRequest);
        if (!response.Items.Any())
        {
            return null;
        }

        var itemAsJson = (Document.FromAttributeMap(response.Items.First())).ToJson();
        var itemAsDto = JsonSerializer.Deserialize<CustomerDto>(itemAsJson);

        return itemAsDto;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        // Should NEVER EVER be implemented using DynamoDb, its costtly and it's not how it is supposed to be used.
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(CustomerDto customer, DateTime requestStartedAt)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);

        // Serializing to DB 
        var customerAsAttribute = Document.FromJson(customerAsJson).ToAttributeMap();

        var updateItemRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = customerAsAttribute,
            ConditionExpression = "UpdatedAt < :requestStartedAt",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":requestStartedAt", new AttributeValue { S = requestStartedAt.ToString("O") } }
            }
        };

        var response = await _dynamoDb.PutItemAsync(updateItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var getUserAttribute = new Dictionary<string, AttributeValue>
        {
            { "pk", new AttributeValue { S = id.ToString() } },
            { "sk", new AttributeValue { S = id.ToString() } }
        };

        var deleteItemRequest = new DeleteItemRequest(_tableName, getUserAttribute);
        var response = await _dynamoDb.DeleteItemAsync(deleteItemRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}