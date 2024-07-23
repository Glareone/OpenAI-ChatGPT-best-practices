using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAI.Embeddings;

namespace ChatGPT.AzureFunction;

public class FormRecognizer
{
    [FunctionName("form-recognizer")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        var docAnalysisClient = new DocumentAnalysisClient(
            new Uri("https://<COGNITIVE_SERVICE_NAME>.cognitiveservices.azure.com/"), 
            new AzureKeyCredential("<PLACE_YOUR_KEY_HERE>"));
        try
        {
            using var memoryStream = new MemoryStream();
            await req.Body.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset the stream position after reading
            
            var docAnalysisOperation = await docAnalysisClient.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-layout", memoryStream, new AnalyzeDocumentOptions() );
            var resultContent = docAnalysisOperation.Value.Content;

            return new OkObjectResult(resultContent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}