using Newtonsoft.Json;
using TelegramBot.Application;

public class BotResponseDataFormatter
{
    [JsonProperty("responses")]
    public Dictionary<string, ResponseContent> Responses { get; } = new();
}