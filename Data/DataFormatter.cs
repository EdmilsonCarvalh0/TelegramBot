using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TelegramBot.Data;

public class DataFormatter
{
    [JsonProperty("item")]
    public string Item { get; set; }

    [JsonProperty("preco")]
    public string Preco { get; set;}
}