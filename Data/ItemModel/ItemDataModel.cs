using Newtonsoft.Json;

namespace TelegramBot.Data;

public abstract class ItemDataModel
{
    [JsonProperty("id")]
    public int Id { get; set; } = 0;

    [JsonProperty("nome")]
    public string Nome { get; set; } = "Não informado";
    
    [JsonProperty("marca")]
    public string Marca { get; set; } = "Não informada";

    [JsonProperty("preco")]
    public decimal Preco { get; set; }
}