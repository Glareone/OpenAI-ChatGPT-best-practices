using System.Text;

if (args.Length > 0)
{
    HttpClient client = new();
    
    client.DefaultRequestHeaders.Add("Authorization", "Bearer <INSERT_YOUT_TOKEN>");

    var content = new StringContent("{\"model\": \"text-davinci-003\", \"prompt\": \"" + args[0] +
                                    "\", \"temperature\": 1, \"max_tokens\": 100}", Encoding.UTF8, "application/json");
    var response = await client.PostAsync("https://api.openai.com/v1/completions", content);

    var responseString = await response.Content.ReadAsStringAsync();
    
    Console.WriteLine(responseString);

    Console.ReadKey();
}
else
{
    Console.WriteLine("--> You need to provide some input");
}