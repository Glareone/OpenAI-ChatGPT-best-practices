using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;

if (args.Length <= 0)
{
    Console.WriteLine("--> You need to provide some input");
    Console.ReadKey();
    return;
}

HttpClient client = new();

client.DefaultRequestHeaders.Add("Authorization", "Bearer <INSERT YOUR TOKEN>");

var content = new StringContent("{\"model\": \"text-davinci-003\", \"prompt\": \"" + args[0] +
                                    "\", \"temperature\": 1, \"max_tokens\": 100}", Encoding.UTF8, "application/json");
var response = await client.PostAsync("https://api.openai.com/v1/completions", content);

var responseString = await response.Content.ReadAsStringAsync();

try
{
    var dynData = JsonConvert.DeserializeObject<dynamic>(responseString);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"> OpenAI Response: {dynData!.choices[0].text}");
    Console.ResetColor();
}
catch (Exception e)
{
    Console.WriteLine($"Could not deserialized Json, error: {e.Message}");
    throw;
}
