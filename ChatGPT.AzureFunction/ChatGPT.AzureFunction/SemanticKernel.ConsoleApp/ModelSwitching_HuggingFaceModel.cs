using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace SemanticKernel.ConsoleApp;

public class ModelSwitching_HuggingFaceModel
{
    public static readonly ChatHistory ChatHistory = new();
    
    private static readonly Lazy<Kernel> Kernel = new (() =>
    {
        var builder = new KernelBuilder();
        builder.AddAzureOpenAIChatCompletion("gpt-4-32k", "gpt-4-32k", "https://ai-proxy.lab.epam.com",
            "<INSERT THE TOKEN HERE>", serviceId: "AzureGtp4TurboService");
        
        var huggingFaceModel = "<NEED TO CHECK THEIR LATEST CONFIGURATION FOR CONNECTOR>";
        #pragma warning disable SKEXP0020
        builder.AddHuggingFaceTextGeneration(huggingFaceModel, apiKey: "<INSERT THE CORRECT API KEY>");
        
        var kernel = builder.Build();
        return kernel;
    });
    
    public async Task<string> InteractWithHuggingFace(string input) {
        var kernel = Kernel.Value;
        
        string skPrompt = @"ChatBot: How can I help you?
        User: {{$input}}
        ---------------------------------------------
        Return data requested by user: ";
        
        var shortIntentFunction = kernel.CreateFunctionFromPrompt(skPrompt);
        var intentResult = await kernel.InvokeAsync(shortIntentFunction, new KernelArguments(input));
        
        // Save new message in the context variables
        ChatHistory.AddUserMessage(input);
    
        Console.WriteLine(intentResult);

        return intentResult.GetValue<string>();
    }
}