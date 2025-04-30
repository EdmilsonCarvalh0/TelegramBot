using System.Globalization;
using Domain.Item;

namespace TelegramBot.DataModels.Item;

public class ShoppingListSnapshot
{
    public string Month { get; private set; }
    public int Day { get; private set; }
    public string Hour { get; private set; }
    public ItemDataFormatter ItemDataFormatter { get; set; }

    public static ShoppingListSnapshot Default => new ShoppingListSnapshot("desconhecido", "--:--") { Day = 0 };

    public ShoppingListSnapshot(string month, string hour)
    {
        Month = month;
        Hour = hour;
        ItemDataFormatter = new ItemDataFormatter();
    }

    public void InitializeFromCurrentTime()
    {
        var now = DateTime.Now;
        Month = now.ToString("MMMM", new CultureInfo("pt-BR"));
        Day = now.Day;
        Hour = now.ToString("HH:mm");
    }

    public override bool Equals(object? o)
    {
        if (o is null) return false;
        if (o is not ShoppingListSnapshot other) return false;
        
        return Month == other.Month &&
               Day == other.Day &&
               Hour == other.Hour;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Month, Day, Hour);
    }
}