using TelegramBot.Domain.Item;

namespace TelegramBot.Infrastructure.Json.JsonStorage;

public class ShoppingHistory
{
    private readonly IJsonFileReader _jsonFileReader;

    public ShoppingHistory(IJsonFileReader jsonFileReader)
    {
        _jsonFileReader = jsonFileReader;
    }

    public ShoppingListSnapshot? GetByMonth(string month)
    {
        var snapshots = _jsonFileReader.ReadFromFile<List<ShoppingListSnapshot>>(FileType.Items);
        
        return snapshots.FirstOrDefault(s => s.Month == month);
    }

    public void SavePurchasedItems(ShoppingListSnapshot snapshot)
    {
        var snapshots = _jsonFileReader.ReadFromFile<List<ShoppingListSnapshot>>(FileType.Items);
        
        snapshots.Add(snapshot);
        
        _jsonFileReader.WriteToFile(FileType.Items, snapshots);
    }
}