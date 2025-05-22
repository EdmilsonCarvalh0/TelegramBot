using System.Globalization;

namespace TelegramBot.DataModels.Item.Snapshot;

public class ShoppingListSnapshot
{
    public ShoppingDateTime ShoppingDateTime { get; private set; } 
    public ItemDataFormatter ItemDataFormatter { get; set; }

    public ShoppingListSnapshot()
    {
        ShoppingDateTime = ShoppingDateTime.GetDefault();
        ItemDataFormatter = new ();
    }

    public void InitializeFromCurrentTime()
    {
        ShoppingDateTime.RegisterDateInfo();
    }
}