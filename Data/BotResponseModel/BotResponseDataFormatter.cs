

using Newtonsoft.Json;
using TelegramBot.UserInterface;

public class BotResponseDataFormatter
{
    [JsonProperty("responses")]
    public Dictionary<string, ResponseContentDTO> Responses { get; } = new();
}