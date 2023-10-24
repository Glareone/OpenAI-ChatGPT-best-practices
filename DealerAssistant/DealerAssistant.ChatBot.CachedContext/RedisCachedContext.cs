using Newtonsoft.Json;
using StackExchange.Redis;
using Entities = DealerAssistant.DAL.Entities;

namespace DealerAssistant.ChatBot.CachedContext;

public class RedisCachedContext
{
    public static async Task<IList<Entities.ChatMessage>?> GetRequestContext(string requestId)
    {
        IDatabase cache = RedisConnection.GetDatabase();
        var recordValue = await cache.StringGetAsync(requestId);
        var isEmpty = string.IsNullOrEmpty(recordValue.ToString());
        
        var context = string.IsNullOrEmpty(recordValue.ToString())
            ? CreateInitialContext()
            : JsonConvert.DeserializeObject<List<Entities.ChatMessage>>(recordValue);

        return context;
    }

    public static async Task<bool> UpdateRequestContext(string requestId, IEnumerable<Entities.ChatMessage> requestContext,
        Entities.ChatMessage[] currentRequestResponse)
    {
        IDatabase cache = RedisConnection.GetDatabase();
        var aggregatedNewValue = requestContext.Concat(currentRequestResponse);
        var serializedNewValue = JsonConvert.SerializeObject(aggregatedNewValue);
        var redisKeyValuePair = new KeyValuePair<RedisKey, RedisValue>(requestId, serializedNewValue);
        return await cache.StringSetAsync(new[] { redisKeyValuePair });
    }

    private static IList<Entities.ChatMessage> CreateInitialContext()
    {
        return new [] { new Entities.ChatMessage("system", InitialContext.BMWCClientContext) };
    }

    private static ConnectionMultiplexer RedisConnection => LazyConnection.Value;

    private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new(() =>
    {
        return ConnectionMultiplexer.Connect("chatgptredis-test.redis.cache.windows.net:6380,password=gFjW8l1boG27L5FjdISt03Ps1DGDdoUEDAzCaBYeSbA=,ssl=True,abortConnect=False");
    });
}