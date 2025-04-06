using System.Globalization;

namespace Domain.Item;

public class Item : ItemDataModel
{
    public static Item Placeholder => new(ItemId.Default, Name.Default, Brand.Default, Price.Default);

    public Item(ItemId id, Name name, Brand brand, Price price)
    {
        Id = id;
        Nome = name;
        Marca = brand;
        Preco = price;
    }

    public override string ToString()
    {
        return $"{Id.ToString()}. {Nome.ToString()} - {Marca.ToString()} - {Preco.ToString()}\n";
    }

    public void SetNome(string attribute)
    {
        Nome.Value = attribute;
    }

    public void SetMarca(string attribute)
    {
        Marca.Value = attribute;
    }

    public void SetPreco(string attribute)
    {
        if (decimal.TryParse(attribute, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
        {
            Preco.Value = result;
        }
    }

}