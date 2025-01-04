using Newtonsoft.Json;

namespace TelegramBot.Data;

public class Item : DataModel
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("nome")]
    public string Nome { get; set; } = "Não informado";
}