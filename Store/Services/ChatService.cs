using Azure;
using Azure.AI.OpenAI;
using Microsoft.IdentityModel.Tokens;
using NRedisStack.Search.Literals.Enums;
using NRedisStack.Search;
using StackExchange.Redis;
using static NRedisStack.Search.Schema;
using NRedisStack.RedisStackCommands;
using NRedisStack;
using DataEntities;
using OpenAI.Chat;
using Azure.AI.OpenAI.Chat;
using NuGet.Protocol;

#pragma warning disable AOAI001
namespace Store.Services
{
    public class ChatService([FromKeyedServices("redisvss")] IConnectionMultiplexer redisConnectionMultiplexer, AzureOpenAIClient aoaiClient, IConfiguration configuration)
    {
        public async Task<string> GetChatResponse(string message)
        {
            ChatClient chatClient = aoaiClient.GetChatClient(configuration.GetConnectionString("gpt4"));

            ChatCompletion response = await chatClient.CompleteChatAsync(message);

            Console.WriteLine("ChatCompletion: ");
            Console.WriteLine(response.ToJson());

            return response.ToJson();


        }
    }
}
