

using Newtonsoft.Json;
using TelegramBot.UserInterface;

public class BotResponseDataFormatter
{
    [JsonProperty("responses")]
    public Dictionary<string, ResponseContent> Responses { get; } = new();
}