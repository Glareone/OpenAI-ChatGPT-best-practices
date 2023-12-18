using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Events;

namespace SemanticKernel.ConsoleApp;

public class SemanticKernelPipeline
{
    public SemanticKernelPipeline(string azureOpenAiKey)
    {
        AzureOpenAiKey = azureOpenAiKey;
    }

    // * Event handlers are established to capture and respond to events during Semantic Kernel execution.
    // These handlers cover prompt rendering, prompt rendered, function invoking, and function invoked events,
    // providing insights into the execution flow.
    private void Kernel_PromptRendering(object? sender, PromptRenderingEventArgs e)
    {
        Console.WriteLine($"[PromptRendering]: \n\t // {e.Function.Description} \n\t {e.Function.Name} ({string.Join(":", e.Arguments)})");
    }
    private void Kernel_PromptRendered(object? sender, PromptRenderedEventArgs e)
    {
        Console.WriteLine($"[PromptRendered]: \n\t{e.RenderedPrompt}");
    }
    private void Kernel_FunctionInvoking(object? sender, FunctionInvokingEventArgs e)
    {
        Console.WriteLine($"[FunctionInvoking]: \n\t // {e.Function.Description} \n\t {e.Function.Name} ({string.Join(":", e.Arguments)})");
    }
    private void Kernel_FunctionInvoked(object? sender, FunctionInvokedEventArgs e)
    {
        Console.WriteLine($"[FunctionInvoked]: \n\t {e.Function.Name}");
    }

    private string AzureOpenAiKey { get; set; }

    public async Task<string> GetBooksAndTranslatePipelineInvoke()
    {
        var builder = new KernelBuilder();
        // register GPT-35
        builder.AddAzureOpenAIChatCompletion("gpt-35-turbo", "gpt-35-turbo", "https://ai-proxy.lab.epam.com",
            AzureOpenAiKey, serviceId: "AzureGtp35TurboService");
        // register GPT-4
        builder.AddAzureOpenAIChatCompletion("gpt-4-32k", "gpt-4-32k", "https://ai-proxy.lab.epam.com",
            AzureOpenAiKey, serviceId: "AzureGtp4TurboService");
        var kernel = builder.Build();

        // Prompts initialization
        // Pay attention we have arguments here
        string findBooksPrompt =
            @"I would sincerely appreciate your assistance in curating a list of the top {{$BooksNumber}} books that explore the fascinating realm of world history. 
                It would be especially helpful if you could suggest books from the period {{$YearFrom}} to {{$YearTo}}. Show just Name and Author.";

        // Translate Prompt.
        // Also has 2 input arguments which will be applied later
        string translatePrompt =
            "Your linguistic expertise is highly valued. Please translate the following text into {{$Lang}}. TEXT: {{$Input}}";

        // Create function for finding top books in world history
        KernelFunction findBooksFunction = kernel.CreateFunctionFromPrompt(
            findBooksPrompt,
            functionName: "TopBooks",
            description: "Retrieves a curated list of top books on world history within a specified time period.");

        // Create function for translating text
        KernelFunction translateFunction = kernel.CreateFunctionFromPrompt(
            translatePrompt,
            functionName: "Translate",
            description: "Translates the provided text into the specified language.");

        // Set up a pipeline of functions to be executed
        var pipeline = new KernelFunction[]
        {
            findBooksFunction,
            translateFunction
        };

        // Set execution settings for OpenAI service
        KernelArguments arguments = new KernelArguments
        {
            ExecutionSettings = new OpenAIPromptExecutionSettings
            {
                ServiceId = "AzureGtp35TurboService",
                Temperature = 0
            }
        };

        // Set specific arguments for the findBooks function
        arguments.Add("BooksNumber", "3");
        arguments.Add("YearFrom", "1950");
        arguments.Add("YearTo", "1960");

        // Set specific arguments for the translate function
        arguments.Add("Lang", "Italian");
        
        // Register Handlers
        kernel.FunctionInvoking += Kernel_FunctionInvoking;
        kernel.FunctionInvoked += Kernel_FunctionInvoked;
        kernel.PromptRendering += Kernel_PromptRendering;
        kernel.PromptRendered += Kernel_PromptRendered;
        
        // * Functions in the pipeline are invoked, and the results are displayed.
        // This section showcases the step-by-step execution of the Semantic Kernel,
        // allowing for a better understanding of its behavior and outputs.
        // Invoke each function in the pipeline and display the output
        var outputs = new List<string>();

        foreach (var item in pipeline)
        {
            var pipelineResult = await kernel.InvokeAsync(item, arguments);
            arguments["Input"] = pipelineResult;
            var valueAsString = pipelineResult.GetValue<string>();
            outputs.Add($"[{pipelineResult.Function.Name}]: {valueAsString}");
        }

        return string.Join(',', outputs);
    }
}