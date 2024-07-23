using System;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAI.Embeddings;

namespace ChatGPT.AzureFunction;

public class Embeddings
{
    [FunctionName("embedding")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        var client = new AzureOpenAIClient(
            new Uri("https://<OPEN_AI_SERVICES_NAME>.openai.azure.com/"),
            new AzureKeyCredential("<PLACE_YOUR_KEY_HERE>"));

        try
        {
            req.Query.TryGetValue("text", out var inputText);
            var embeddingObject = await client.GetEmbeddingClient("embedding-3-large").GenerateEmbeddingAsync(inputText,
                new EmbeddingGenerationOptions
                {
                    Dimensions = 1536
                });

            return new OkObjectResult(JsonConvert.SerializeObject(embeddingObject.Value.Vector.ToArray()));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}