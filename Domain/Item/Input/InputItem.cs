using System.Globalization;

namespace TelegramBot.Domain.Item;

public class InputItem
{
    public string Name { get; private set; } = string.Empty;
    public string Brand  { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    private List<string> _itemAttributes = new();
    private List<string> ItemAttributes{ 
        get => _itemAttributes;
        set
        {
            _itemAttributes = value;
            ValidateItems();
        }
        
    }

    public InputItem(List<string> linesWithUnprocessedItems)
    {
        ItemAttributes = linesWithUnprocessedItems;
    }

    private void ValidateItems()
    {
        if (!CheckIfIsCompleteItem(_itemAttributes))
        {
            throw new ArgumentException("Forneça um item com Nome, Marca e Preço.");
        }

        if (!decimal.TryParse(_itemAttributes[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
        {
            throw new ArgumentException("Forneça um preço válido.");
        }
        
        LoadAttributes(price);
    }
    
    private static bool CheckIfIsCompleteItem(List<string> linesOfItems) => linesOfItems.Count >= 3;

    private void LoadAttributes(decimal price)
    {
        Name = _itemAttributes[0];
        Brand = _itemAttributes[1];
        Price = price;
    }
}