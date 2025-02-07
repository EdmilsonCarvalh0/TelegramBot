using Newtonsoft.Json;
using TelegramBot.Application;

namespace TelegramBot.Data;

public abstract class BotResponseDataModel
{
    [JsonProperty]
    public string ResponseRequest { get; set; } = string.Empty;

    [JsonProperty]
    public ResponseContentDTO ResponseContent { get; } = new ();
}