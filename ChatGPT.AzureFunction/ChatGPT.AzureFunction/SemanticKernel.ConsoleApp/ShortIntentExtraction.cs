using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

namespace SemanticKernel.ConsoleApp;

public static class ShortIntentExtraction
{
    public static async Task<(string gpt35result, string gpt4result)> InvokeEpamSemanticKernelEndpoint()
    {
        var builder = new KernelBuilder();
        // register GPT-35
        builder.AddAzureOpenAIChatCompletion("gpt-35-turbo", "gpt-35-turbo", "https://ai-proxy.lab.epam.com", "<INSERT YOUR KEY>", serviceId: "AzureGtp35TurboService");
        // register GPT-4
        builder.AddAzureOpenAIChatCompletion("gpt-4-32k", "gpt-4-32k", "https://ai-proxy.lab.epam.com", "<INSERT YOUR KEY>", serviceId: "AzureGtp4TurboService");
        var kernel = builder.Build();

        // Request GPT3.5 Configuration
        var aiRequestSettingsGpt35 = new OpenAIPromptExecutionSettings 
        {
            ExtensionData = new Dictionary<string, object> { { "api-version", "2023-03-15-preview" } },
            ServiceId = "AzureGtp35TurboService"
        };
        
        // Request GPT4 Configuration
        var aiRequestSettingsGpt4 = new OpenAIPromptExecutionSettings 
        {
            ExtensionData = new Dictionary<string, object> { { "api-version", "2023-03-15-preview" } },
            ServiceId = "AzureGtp4TurboService"
        };

        // Prompt
        var UserInput = "I want to find top-10 books about world history";
        var SemanticKernelPrompt = @"ChatBot: How can I help you?
                User: {{$input}}
                ---------------------------------------------
                Return data requested by user: ";
                
        var getShortIntentFunctionGpt35  = kernel.CreateFunctionFromPrompt(SemanticKernelPrompt, aiRequestSettingsGpt35);
        var intentResultGpt35 = await kernel.InvokeAsync(getShortIntentFunctionGpt35, new KernelArguments(UserInput));
        
        var getShortIntentFunctionGpt4  = kernel.CreateFunctionFromPrompt(SemanticKernelPrompt, aiRequestSettingsGpt4);
        var intentResultGpt4 = await kernel.InvokeAsync(getShortIntentFunctionGpt4, new KernelArguments(UserInput));

        if (string.IsNullOrEmpty(intentResultGpt35?.GetValue<string>()) &&
            string.IsNullOrEmpty(intentResultGpt4?.GetValue<string>()))
        {
            return ("Result From GPT3.5 is empty", "Result from GPT4 is empty");
        }
        
        return (intentResultGpt35.GetValue<string>(), intentResultGpt4.GetValue<string>());
    }
}