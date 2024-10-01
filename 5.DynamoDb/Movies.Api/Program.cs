using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Movies.Api;

await new DataSeeder().ImportDataAsync();

var newMovie1 = new Movie1
{
    Id = Guid.NewGuid(),
    Title = "21 Jump Street",
    AgeRestriction = 18,
    ReleaseYear = 2012,
    RottenTomatoesPercentage = 85
};


var newMovie2 = new Movie2
{
    Id = Guid.NewGuid(),
    Title = "21 Jump Street",
    AgeRestriction = 18,
    ReleaseYear = 2012,
    RottenTomatoesPercentage = 85
};

var asJson1 = JsonSerializer.Serialize(newMovie1);
var attributeMap1 = Document.FromJson(asJson1).ToAttributeMap();

var asJson2 = JsonSerializer.Serialize(newMovie2);
var attributeMap2 = Document.FromJson(asJson2).ToAttributeMap();

// Transactional request with 2 puts
var transactionRequest = new TransactWriteItemsRequest
{
    TransactItems = new List<TransactWriteItem>
    {
        new TransactWriteItem
        {
            Put = new Put
            {
                TableName = "movies-yeat-title",
                Item = attributeMap1
            },
        },
        new TransactWriteItem
        {
            Put = new Put
            {
                TableName = "movies-tile-rotten",
                Item = attributeMap2
            }
        }
    }
};

var dynamoDb = new AmazonDynamoDBClient();

var response = await dynamoDb.TransactWriteItemsAsync(transactionRequest);

var msg = response.HttpStatusCode == HttpStatusCode.OK
    ? "Items saved transactionally"
    : "Error saving items transactionally";

