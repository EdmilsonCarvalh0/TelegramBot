using Newtonsoft.Json;

namespace Domain.Item;

public abstract class ItemDataModel
{
    [JsonProperty("id")]
    public ItemId Id { get; set; } = ItemId.Default;

    [JsonProperty("nome")]
    public Name Nome { get; set; } = Name.Default;

    [JsonProperty("marca")]
    public Brand Marca { get; set; } = Brand.Default;

    [JsonProperty("preco")]
    public Price Preco { get; set; } = Price.Default;
}