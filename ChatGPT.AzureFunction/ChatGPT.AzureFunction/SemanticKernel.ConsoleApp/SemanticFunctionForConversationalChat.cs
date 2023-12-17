using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace SemanticKernel.ConsoleApp;

public static class SemanticFunctionForConversationalChat
{
    public static async Task<(string resultFromGpt35, string resultFromGpt4)> Invoke()
    {
        var builder = new KernelBuilder();
        // register GPT-35
        builder.AddAzureOpenAIChatCompletion("gpt-35-turbo", "gpt-35-turbo", "https://ai-proxy.lab.epam.com", "f668fd11763c428bb3bddf08c6d7e412", serviceId: "AzureGtp35TurboService");
        // register GPT-4
        builder.AddAzureOpenAIChatCompletion("gpt-4-32k", "gpt-4-32k", "https://ai-proxy.lab.epam.com", "<INSERT YOUR KEY>", serviceId: "AzureGtp4TurboService");
        var kernel = builder.Build();
        
        // Chat History (obviously)
        var chatHistory = new ChatHistory("You are a librarian, expert about books");
        var message = chatHistory.Last();
        
        chatHistory.AddUserMessage("Hi, I'm looking for book suggestions");
        message = chatHistory.Last();
        Console.WriteLine($"{message.Role}: {message.Content}");
        
        // Get The Chat Content using SemanticKernel
        IChatCompletionService chatService = kernel.GetRequiredService<IChatCompletionService>("AzureGtp35TurboService");
        var gpt35reply = await chatService.GetChatMessageContentAsync(chatHistory);
        Console.WriteLine(gpt35reply);
        
        // Play a bit with Chat History
        message = chatHistory.Last();
        Console.WriteLine($"{message.Role}: {message.Content}");
        chatHistory.AddSystemMessage(gpt35reply);
        message = chatHistory.Last();
        Console.WriteLine($"{message.Role}: {message.Content}");
        chatHistory.AddUserMessage("probably, sci-fi");
        
        // get the chat content using GPT4
        IChatCompletionService chatServiceGpt4 = kernel.GetRequiredService<IChatCompletionService>("AzureGtp4TurboService");
        var gpt4reply = await chatServiceGpt4.GetChatMessageContentAsync(chatHistory);
        
        return (gpt35reply, gpt4reply);
    }
}