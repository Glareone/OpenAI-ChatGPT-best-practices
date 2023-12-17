using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace SemanticKernel.ConsoleApp;

public class InteractiveChatFunction
{
    public static readonly ChatHistory ChatHistory = new();

    private static readonly Lazy<Kernel> Kernel = new (() =>
    {
        var builder = new KernelBuilder();
        builder.AddAzureOpenAIChatCompletion("gpt-4-32k", "gpt-4-32k", "https://ai-proxy.lab.epam.com",
            "<INSERT THE TOKEN HERE>", serviceId: "AzureGtp4TurboService");
        var kernel = builder.Build();
        return kernel;
    });
    
    public async Task<string> Chat (string input) {
        var kernel = Kernel.Value;
        IChatCompletionService chatService = kernel.GetRequiredService<IChatCompletionService>("AzureGtp4TurboService");
        
        // Save new message in the context variables
        ChatHistory.AddUserMessage(input);

        var reply = await chatService.GetChatMessageContentAsync(ChatHistory);

        // Save the answer
        ChatHistory.AddSystemMessage(reply);
    
        Console.WriteLine(reply);

        return reply;
    }
}