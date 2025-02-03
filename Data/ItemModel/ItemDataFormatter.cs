using TelegramBot.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TelegramBot.Data;

public class ItemDataFormatter
{
    [JsonProperty("items")]
    public List<Item> Items { get; set; } = new();

}