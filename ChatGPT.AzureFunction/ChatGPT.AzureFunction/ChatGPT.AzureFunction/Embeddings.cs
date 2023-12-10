using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;

namespace ChatGPT.AzureFunction;

public static class Embeddings
{
    [FunctionName("embeddings")]
    public static async Task<IActionResult> RunAsync()
    {
        await Task.Delay(1500);
        return new OkResult();
    }
}