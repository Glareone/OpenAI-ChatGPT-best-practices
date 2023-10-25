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

public static class ChatGptEmailAutoReplyChatCompletions
{
    private const string DefaultChatGptModel = "gpt-3.5-turbo-16k-0613";
    private const decimal DefaultChatGptTemperature = 0.5m;

    private const string EmailAutoReplyContext = "I am experienced software engineer and I am writing on C#, Golang, Javascript. I receive lots of proposals to different positions." +
                                                 "Technologies close to my stack are the following: React, NextJS, Azure, AWS, C# MinimalAPI, Kafka, Microservices, Solution Architecture." +
                                                 "I consider Lead positions and higher only. If proposed position is for Senior developer or lower - greet the client from whom this email came, say that you are my AI-assistant and politely reject the proposal. If you see that in email customer mentioned any of technologies, then greet the customer and say that I will get back to him soon personally.";

    [FunctionName("email-autoreply-chatcompletion")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        var environmentValue = Environment.GetEnvironmentVariable("chatGPTToken");

        if (string.IsNullOrWhiteSpace(environmentValue))
        {
            return new BadRequestObjectResult("chatGPT token is not provided");
        }

        if (!req.Query.TryGetValue("prompt", out var prompts) || prompts.Count == 0)
        {
            log.LogError("C# HTTP trigger function stops processing the call because prompt is corrupted or not provided");
            return new BadRequestObjectResult("prompt is not provided");
        }

        var isTemperatureDeclared = req.Query.TryGetValue("temperature", out var temperatures);

        HttpClient client = new();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {environmentValue}");

        var context = new ChatMessage("user", EmailAutoReplyContext);
        var message = new ChatMessage("user", prompts[0]);

        dynamic content = new ExpandoObject();
        content.model = DefaultChatGptModel;
        content.messages = new[] { context, message };
        content.temperature = isTemperatureDeclared && Decimal.TryParse(temperatures[0], out var temperature) ? temperature : DefaultChatGptTemperature;
        content.max_tokens = 16000;

        var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", stringContent);

        var responseString = await response.Content.ReadAsStringAsync();

        try
        {
            var dynData = JsonConvert.DeserializeObject<dynamic>(responseString);
            var answer = (string)dynData.choices[0].message.content;
            return new OkObjectResult(answer.Replace("\n", ""));
        }
        catch (Exception e)
        {
            log.LogInformation("Could not deserialized Json, error: {0}", e.Message);
            return new BadRequestObjectResult($"answer could not be deserialized. {e.Message}");
        }
    }
    
    internal record ChatMessage(string role, string content);
}