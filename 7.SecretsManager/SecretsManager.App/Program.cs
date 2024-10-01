// See https://aka.ms/new-console-template for more information

using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

var secretManagerClient = new AmazonSecretsManagerClient();

var request = new GetSecretValueRequest();
request.SecretId = "ApiKey";

var response = await secretManagerClient.GetSecretValueAsync(request);

Console.WriteLine(response.SecretString);
