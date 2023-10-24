using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DealerAssistant.ChatBot.Startup))]
namespace DealerAssistant.ChatBot
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var requiredEnvironmentVariables = new List<string> { "chatGPTToken", "ChatGPTModel", "ChatGPTTemperature", "MaxTokens", "RedisCacheConnectionString" };
            var configuration = builder.GetContext().Configuration;

            foreach(var variable in requiredEnvironmentVariables)
            {
                if(string.IsNullOrEmpty(configuration[variable]))
                {
                    throw new InvalidOperationException($"Environment variable for {variable} is not set");
                }
            }
        }
    }
}