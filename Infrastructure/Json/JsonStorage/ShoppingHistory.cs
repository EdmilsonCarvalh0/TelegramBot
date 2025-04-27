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
        var snapshots = GetSnapShots();
        
        snapshots.Add(snapshot);
        
        _jsonFileReader.WriteToFile(FileType.Items, snapshots);
    }

    public void SavingChangedSnapshot(ShoppingListSnapshot snapshotToSave)
    {
        var snapshots = GetSnapShots();
        
        var index = snapshots.FindIndex(s=> s.Equals(snapshotToSave));
        snapshots[index] = snapshotToSave;
        
        _jsonFileReader.WriteToFile(FileType.Items, snapshots);
    }

    private List<ShoppingListSnapshot> GetSnapShots()
    {
        return _jsonFileReader.ReadFromFile<List<ShoppingListSnapshot>>(FileType.Items);
    }
}