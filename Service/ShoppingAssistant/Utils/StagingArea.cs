using Domain.Item;
using TelegramBot.Domain.Item;
using TelegramBot.Domain.Item.Input;
using TelegramBot.Infrastructure.Json;
using TelegramBot.Infrastructure.Json.JsonStorage;

namespace TelegramBot.Service.ShoppingAssistant.Utils;

public class StagingArea : IStagingArea
{
    private List<string> _itemsToBuy;
    private List<InputItem> _purchasedInputItems;
    private readonly ItemDataFormatter _purchasedItems;
    private InputItem? _itemNotListed;

    public StagingArea(ShoppingHistory shoppingHistory)
    {
        _purchasedInputItems = new();
        _purchasedItems = new();
        _itemNotListed = null;
        _itemsToBuy = new();
    }

    public List<InputItem> GetPurchasedInputItems() => _purchasedInputItems;

    public void SaveNewInputItems(List<InputItem> inputItems)
    {
        inputItems.ForEach(item => _purchasedInputItems.Add(item));
    }
    
    public List<string> GetItemsToBuy() => _itemsToBuy;

    public void UpdateListOfItemsToBuy(string item)
    {
        _itemsToBuy.Remove(item);
    }
    
    public InputItem? GetItemNotListed() => _itemNotListed;

    public void ReserveItemNotListed(InputItem item)
    {
        _itemNotListed = item;
    }

    public void SaveItemNotListed()
    {
        _purchasedInputItems.Add(_itemNotListed!);
    }
    
    public void SaveItemsToBuy(List<string> items)
    {
        _itemsToBuy.AddRange(items);
    }
}