using Newtonsoft.Json;

namespace TelegramBot.Data;

public abstract class DataModel
{
    [JsonProperty("marca")]
    public string Marca { get; set; } = "Não informada";

    [JsonProperty("preco")]
    public decimal Preco { get; set; }
}