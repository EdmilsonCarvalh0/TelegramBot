using TelegramBot.Domain.ItemEntity;

namespace TelegramBot.DataModels.Item.Snapshot;

public class ShoppingListSnapshot
{
    public ShoppingDateTime ShoppingDateTime { get; private set; } 
    public decimal Total { get; private set; }
    public ItemList ItemCollection { get; set; }

    public ShoppingListSnapshot()
    {
        ShoppingDateTime = ShoppingDateTime.GetDefault();
        ItemCollection = new();
    }

    public void InitializeFromCurrentTime()
    {
        ShoppingDateTime.RegisterDateInfo();
    }

    public void IncreaseTotal(decimal value)
    {
        Total += value;
    }
}