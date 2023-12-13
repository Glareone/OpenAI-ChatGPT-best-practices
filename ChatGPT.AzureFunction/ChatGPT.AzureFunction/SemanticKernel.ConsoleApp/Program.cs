using SemanticKernel.ConsoleApp;

var (intentResultGpt35, intentResultGpt4) = await ShortIntentExtraction.InvokeEpamSemanticKernelEndpoint();

Console.WriteLine(intentResultGpt35);
Console.WriteLine(intentResultGpt4);
