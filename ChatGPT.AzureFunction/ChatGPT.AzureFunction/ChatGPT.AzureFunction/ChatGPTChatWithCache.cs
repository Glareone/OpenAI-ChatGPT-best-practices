using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace ChatGPT.AzureFunction;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Dynamic;

public static class ChatGPTChatWithCache
{
    private const string DefaultChatGptModel = "gpt-3.5-turbo-16k-0613";
    private const decimal DefaultChatGptTemperature = 0.5m;

    private static ILogger Log;

    [FunctionName("chat-completion-with-cache")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        Log = log;
        var environmentValue = Environment.GetEnvironmentVariable("chatGPTToken");
        var requestId = string.Empty;

        if (string.IsNullOrWhiteSpace(environmentValue))
        {
            return new BadRequestObjectResult("chatGPT token is not provided");
        }

        if (!req.Query.TryGetValue("prompt", out var prompts) || prompts.Count == 0)
        {
            log.LogError("C# HTTP trigger function stops processing the call because prompt is corrupted or not provided");
            return new BadRequestObjectResult("prompt is not provided");
        }

        if (!req.Query.TryGetValue("request-id", out var requestIds) || requestIds.Count == 0)
        {
            log.LogInformation("Request does not provide unique requestId, requestId was generated automatically");
            requestId = Guid.NewGuid().ToString();
        }

        // Get Request Context which consist of questions and answers from previous
        var requestContext = await GetRequestContext(requestId);
        var serializedRequestContext = JsonConvert.DeserializeObject<ChatMessage[]>(requestContext) ?? Array.Empty<ChatMessage>();

        // Compose Message to ChatGPT and Prepare the client
        HttpClient client = new();
        var isModelDeclared = req.Query.TryGetValue("model", out var models);
        var isTemperatureDeclared = req.Query.TryGetValue("temperature", out var temperatures);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {environmentValue}");
        var message = new ChatMessage("user", prompts[0]);

        dynamic content = new ExpandoObject();
        content.model = isModelDeclared && !string.IsNullOrEmpty(models[0]) ? models[0] : DefaultChatGptModel;
        content.messages = serializedRequestContext.Concat(new ChatMessage[] { message });
        content.temperature = isTemperatureDeclared && Decimal.TryParse(temperatures[0], out var temperature) ? temperature : DefaultChatGptTemperature;
        content.max_tokens = 3000;

        var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", stringContent);

        var responseString = await response.Content.ReadAsStringAsync();

        try
        {
            var dynData = JsonConvert.DeserializeObject<dynamic>(responseString);
            var answer = (string)dynData.choices[0].message.content;
            
            // Update Requested Context
            var serializedAnswer = new ChatMessage ("system", answer);
            await UpdateRequestContext(
                requestId,
                serializedRequestContext,
                new ChatMessage[] { message, serializedAnswer });
            
            return new OkObjectResult(answer.Replace("\n", ""));
        }
        catch (Exception e)
        {
            log.LogInformation("Could not deserialized Json, error: {0}", e.Message);
            return new BadRequestObjectResult($"answer could not be deserialized. {e.Message}");
        }
    }

    private static async Task<string> GetRequestContext(string requestId)
    {
        try
        {
            IDatabase cache = RedisConnection.GetDatabase();
            var recordValue = await cache.StringGetAsync(requestId);
            return recordValue.ToString();
        }
        catch (Exception e)
        {
            Log.LogInformation("Redis database cannot be read, error: {0}", e.Message);
            return string.Empty;
        }
    }

    private static async Task<bool> UpdateRequestContext(string requestId, ChatMessage[] requestContext, ChatMessage[] currentRequestResponse)
    {
        try
        {
            IDatabase cache = RedisConnection.GetDatabase();
            var aggregatedNewValue = requestContext.Concat(currentRequestResponse);
            var serializedNewValue = JsonConvert.SerializeObject(aggregatedNewValue);
            var redisKeyValuePair = new KeyValuePair<RedisKey, RedisValue>(requestId, serializedNewValue);
            return await cache.StringSetAsync(new []{ redisKeyValuePair });
        }
        catch (Exception e)
        {
            Log.LogInformation("Redis database cannot be written, error: {0}", e.Message);
            return false;
        }
    }
    
    private static ConnectionMultiplexer RedisConnection => LazyConnection.Value;

    private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new (() =>
    {
        var cacheConnection = Environment.GetEnvironmentVariable("redisCacheConnectionString");
        return ConnectionMultiplexer.Connect(cacheConnection);
    });

    internal record ChatMessage(string role, string content);
}