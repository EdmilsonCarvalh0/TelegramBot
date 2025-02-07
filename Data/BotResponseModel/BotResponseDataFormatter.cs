using Newtonsoft.Json;
using TelegramBot.Application;

public class BotResponseDataFormatter
{
    [JsonProperty("responses")]
    public Dictionary<string, ResponseContentDTO> Responses { get; } = new();
}