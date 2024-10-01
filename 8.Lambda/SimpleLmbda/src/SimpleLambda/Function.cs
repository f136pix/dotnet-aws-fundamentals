using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SimpleLmbda;

public class Function
{
   public void FunctionHandler(string input, ILambdaContext context)
    {
        context.Logger.LogInformation("Hello from C#");
    }
}