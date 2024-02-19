using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Kernel = Microsoft.SemanticKernel.Kernel;

namespace SemanticKernel.AzureFunction;

public static class SemanticKernelDataExtension
{
    [FunctionName("semantic-kernel-gpt35-data-extension")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        var apiKey = Environment.GetEnvironmentVariable("chatGPTToken");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return new BadRequestObjectResult("chatGPT token is not provided");
        }

        if (!req.Query.TryGetValue("prompt", out var prompts) || prompts.Count == 0)
        {
            log.LogError("C# HTTP trigger function stops processing the call because prompt is corrupted or not provided");
            return new BadRequestObjectResult("prompt is not provided");
        }
        
        var builder = new KernelBuilder();
        
        // GPT-3.5 Model Attached
        builder.AddAzureOpenAIChatCompletion("gpt-35-turbo", "gpt-3.5-turbo-1106", "https://ai-proxy.lab.epam.com", apiKey, serviceId: "AzureGtp35TurboService");
        
        // GPT-4 Model Attached
        builder.AddAzureOpenAIChatCompletion(deploymentName: "gpt-4-32k", "gpt-4-32k", "https://ai-proxy.lab.epam.com", apiKey, serviceId: "AzureGtp4TurboService");
        
        
        var kernel = builder.Build();
        
        var aiRequestSettings = new OpenAIPromptExecutionSettings 
        {
            ExtensionData = new Dictionary<string, object> { { "api-version", "2023-03-15-preview" } },
            ServiceId = "AzureGtp35TurboService"
        };
        
        var input = "I want to find top-10 books about world history";
        
        string skPrompt = @"ChatBot: How can I help you?
                            User: {{$input}}
                            ---------------------------------------------
                            Return data requested by user: ";
        
        var getShortIntentFunction  = kernel.CreateFunctionFromPrompt(skPrompt, aiRequestSettings);

        var intentResult = await kernel.InvokeAsync(getShortIntentFunction, new KernelArguments(input));

        Console.WriteLine(intentResult);

        return new OkObjectResult(intentResult);
        
        // HttpClient client = new();
        // client.DefaultRequestHeaders.Add("Api-key", environmentValue);
        // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //
        // dynamic content = new ExpandoObject();
        // content.max_tokens = 3000;
        // content.temperature = 0.7;
        // content.messages = new object[]
        // {
        //     new { role = "system", content = prompts[0] }
        // };
        
        // var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        // var response = await client.PostAsync("https://ai-proxy.lab.epam.com/openai/deployments/gpt-4/chat/completions?api-version=2023-06-01-preview", stringContent);
        // var responseString = await response.Content.ReadAsStringAsync();
        //
        // try
        // {
        //     var convertedResponse = JsonConvert.DeserializeObject<Response>(responseString);
        //     var answer = convertedResponse.choices[0].message.content;
        //     return new OkObjectResult(answer.Replace("\n", ""));
        // }
        // catch (Exception e)
        // {
        //     log.LogInformation("Could not deserialized Json, error: {0}", e.Message);
        //     return new BadRequestObjectResult("answer could not be deserialized");
        // }
    }
    
    private record Response(string id, int created, string model, Choices[] choices);
    private record Choices(int index, Message message);
    private record Message(string role, string content);
}