using SemanticKernel.ConsoleApp;

var (intentResultGpt35, intentResultGpt4) = await ShortIntentExtraction.InvokeEpamSemanticKernelEndpoint();

Console.WriteLine(intentResultGpt35);
Console.WriteLine(intentResultGpt4);

var (gpt35content, gpt4content) = await SemanticFunctionForConversationalChat.Invoke();
Console.WriteLine(gpt35content);
Console.WriteLine(gpt4content);
