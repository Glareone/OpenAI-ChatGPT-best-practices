using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DealerAssistant.ChatBot.CachedContext;
using DealerAssistant.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DealerAssistant.ChatBot;

public class SelectACarAssistanceRequest
{
    private static ILogger _log;

    [FunctionName("chat-completion-with-cache")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        _log = log;
        var requestorId = string.Empty;

        if (!req.Query.TryGetValue("prompt", out var prompts) || prompts.Count == 0)
        {
            log.LogError("C# HTTP trigger function stops processing the call because prompt is corrupted or not provided");
            return new BadRequestObjectResult("Hello! It's BMW AI-assistant. I am waiting to help you.");
        }

        if (!req.Query.TryGetValue("requestorid", out var requestorIds) || requestorIds.Count == 0)
        {
            log.LogInformation("Request does not provide unique requestId, requestId was generated automatically");
            requestorId = Guid.NewGuid().ToString();
        }
        else
        {
            requestorId = requestorIds[0];
        }

        // Get Request Context which consist of questions and answers from previous
        var requestContext = await GetRequestContext(requestorId); ;

        // Compose Message to ChatGPT and Prepare the client
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Environment.GetEnvironmentVariable("chatGPTToken")}");
        var message = new Entities.ChatMessage("user", prompts[0]);

        dynamic content = new ExpandoObject();
        content.model = Environment.GetEnvironmentVariable("ChatGPTModel");
        content.messages = requestContext.Concat(new [] { message });
        content.temperature =  decimal.Parse(Environment.GetEnvironmentVariable("ChatGPTTemperature") ?? "0");;
        content.max_tokens = int.Parse(Environment.GetEnvironmentVariable("MaxTokens") ?? "1");

        var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", stringContent);
        var responseString = await response.Content.ReadAsStringAsync();

        try
        {
            var dynData = JsonConvert.DeserializeObject<dynamic>(responseString);
            var answer = (string)dynData.choices[0].message.content;
            
            // Update Requested Context
            var serializedAnswer = new Entities.ChatMessage("system", answer);
            await UpdateRequestContext(
                requestorId,
                requestContext,
                new [] { message, serializedAnswer });
            
            return new OkObjectResult(answer.Replace("\n", ""));
        }
        catch (Exception e)
        {
            log.LogInformation("Could not deserialized Json, error: {0}", e.Message);
            return new BadRequestObjectResult($"answer could not be deserialized. {e.Message}");
        }
    }

    private static async Task<IList<Entities.ChatMessage>> GetRequestContext(string requestId)
    {
        try
        {
            var context = await RedisCachedContext.GetRequestContext(requestId);
            return context;
        }
        catch (Exception e)
        {
            _log.LogInformation("Redis database cannot be read, error: {0}", e.Message);
            return new List<Entities.ChatMessage>();
        }
    }
    
    private static async Task<bool> UpdateRequestContext(string requestId, IEnumerable<Entities.ChatMessage> requestContext, Entities.ChatMessage[] currentRequestResponse)
    {
        try
        {
            await RedisCachedContext.UpdateRequestContext(requestId, requestContext, currentRequestResponse);
            return true;
        }
        catch (Exception e)
        {
            _log.LogInformation("Redis database cannot be written, error: {0}", e.Message);
            return false;
        }
    }
}