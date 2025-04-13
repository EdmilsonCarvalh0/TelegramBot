using Newtonsoft.Json;
using TelegramBot.Application.Bot;

namespace TelegramBot.Infrastructure.JsonStorage;

public class BotResponseCollection
{
    [JsonProperty("responses")]
    public Dictionary<string, ResponseContent> Responses { get; } = new();
}