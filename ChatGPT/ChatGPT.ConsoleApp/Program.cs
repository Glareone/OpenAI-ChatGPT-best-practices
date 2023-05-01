using System.Text;
using Newtonsoft.Json;

if (args.Length <= 0)
{
    Console.WriteLine("--> You need to provide some input");
    Console.ReadKey();
    return;
}

HttpClient client = new();

client.DefaultRequestHeaders.Add("Authorization", "Bearer <INSERT YOUR OPENAPI TOKEN HERE>");

var content = new StringContent("{\"model\": \"text-davinci-003\", \"prompt\": \"" + args[0] +
                                    "\", \"temperature\": 1, \"max_tokens\": 100}", Encoding.UTF8, "application/json");
var response = await client.PostAsync("https://api.openai.com/v1/completions", content);

var responseString = await response.Content.ReadAsStringAsync();

try
{
    var dynData = JsonConvert.DeserializeObject<dynamic>(responseString);

    string guessCommand = GuessCommand(dynData!.choices[0].text);
    
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"> chatGPT guess at command prompt is: {guessCommand}");
    Console.ResetColor();
}
catch (Exception e)
{
    Console.WriteLine($"Could not deserialized Json, error: {e.Message}");
    throw;
}

static string GuessCommand(string openAPIresponse)
{
    Console.WriteLine("--> GPT-3 Davinci returned");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(openAPIresponse);

    var lastIndex = openAPIresponse.LastIndexOf('\n');

    var guess = openAPIresponse[(lastIndex + 1)..];
    
    Console.ResetColor();
    
    TextCopy.ClipboardService.SetText(guess);

    return guess;
}