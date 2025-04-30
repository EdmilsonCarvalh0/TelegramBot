using System.Globalization;

namespace TelegramBot.DataModels.Item.Snapshot;

public class ShoppingListSnapshot
{
    public ShoppingDateTime ShoppingDateTime { get; } 
    public ItemDataFormatter ItemDataFormatter { get; set; }

    public ShoppingListSnapshot()
    {
        ShoppingDateTime = ShoppingDateTime.Default;
        ItemDataFormatter = new ();
    }

    public void InitializeFromCurrentTime()
    {
        ShoppingDateTime.RegisterDateInfo();
    }
}