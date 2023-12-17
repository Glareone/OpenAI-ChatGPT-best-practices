using SemanticKernel.ConsoleApp;

var (intentResultGpt35, intentResultGpt4) = await ShortIntentExtraction.InvokeEpamSemanticKernelEndpoint();

Console.WriteLine(intentResultGpt35);
Console.WriteLine(intentResultGpt4);

// Semantic Kernel. Function For Conversational Chat (trivial example)
var (gpt35content, gpt4content) = await SemanticFunctionForConversationalChat.Invoke();
Console.WriteLine(gpt35content);
Console.WriteLine(gpt4content);

// Semantic Kernel. Interactive Chat Using Function
InteractiveChatFunction interactiveChatFunction = new();
var answer =
    await interactiveChatFunction.Chat(
        "I would like a non-fiction book suggestion about Greece history. Please only list one book.");
Console.WriteLine(answer);
answer = await interactiveChatFunction.Chat(
    "that sounds interesting, what are some of the topics I will learn about?");
Console.WriteLine(answer);
answer = await interactiveChatFunction.Chat(
    "that sounds interesting, what are some of the topics I will learn about?");
Console.WriteLine(answer);
answer = await interactiveChatFunction.Chat(
    "Which topic from the ones you listed do you think the most popular?");
Console.WriteLine(answer);

foreach (var ChatHistoryMessage in InteractiveChatFunction.ChatHistory)
{
    Console.WriteLine($"{ChatHistoryMessage.Role}: {ChatHistoryMessage.Content}");
}