using TelegramBot.DataModels.Item.Snapshot;

namespace TelegramBot.Application.Handlers.ListViewAccess.Utils;

public class DateSelectionCache
{
    private List<ShoppingDateTime> _selectionCache = [];

    public void RegisterDates(List<ShoppingDateTime> shoppingDateTimes)
    {
        _selectionCache = shoppingDateTimes;
    }

    public ShoppingDateTime GetDataInCollection(List<string> infoChosenDate)
    {
        var chosenDate = LoadObjectOfComparison(infoChosenDate);

        var sdt = _selectionCache.FirstOrDefault(sdt => sdt.Equals(chosenDate))!
            ?? throw new ArgumentException("A data não foi encontrada, certifique-se de registrar as datas enviadas com o método RegisterDates() antes de consultar a data na coleção.");

        return sdt;
    }

    private ShoppingDateTime LoadObjectOfComparison(List<string> infoChosenDate)
    {
        return new ShoppingDateTime(infoChosenDate[1].ToLower(), Convert.ToInt32(infoChosenDate[0]), infoChosenDate[2]);
    }
}