using System.Globalization;
using Domain.Item;

namespace TelegramBot.Domain.Item;

public class ShoppingListSnapshot
{
    public string Month { get; set; }
    public int Day { get; set; }
    public string Hour { get; set; }
    public ItemDataFormatter ItemDataFormatter { get; set; }

    public ShoppingListSnapshot()
    {
        var now = DateTime.Now;
        Month = now.ToString("MMMM", new CultureInfo("pt-BR"));
        Day = now.Day;
        Hour = now.ToString("HH:mm");
        ItemDataFormatter = new ItemDataFormatter();
    }
}