using TelegramBot.DataModels.Item.Snapshot;

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
        var snapshots = GetSnapShots();
        return snapshots.FirstOrDefault(s => s.ShoppingDateTime.Month == month);
    }

    public List<ShoppingDateTime> GetAllTheDatesFromTheExistingLists()
    {
        var snapshots = GetSnapShots();
        return snapshots.Select(s => s.ShoppingDateTime).ToList();
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