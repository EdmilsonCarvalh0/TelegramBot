using TelegramBot.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TelegramBot.Data;

public class DataFormatter
{
    [JsonProperty("items")]
    public List<Item> Items { get; set; } = new();

}