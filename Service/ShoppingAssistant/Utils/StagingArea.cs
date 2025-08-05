using TelegramBot.Domain.Item.Input;
using TelegramBot.Domain.ItemEntity;
using TelegramBot.Infrastructure.Json.JsonStorage;

namespace TelegramBot.Service.ShoppingAssistant.Utils;

public class StagingArea : IStagingArea
{
    private List<string> _itemsToBuy;
    private List<ItemInput> _purchasedInputItems;
    private readonly ItemList _purchasedItems;
    private ItemInput? _itemNotListed;

    public StagingArea(ShoppingHistory shoppingHistory)
    {
        _purchasedInputItems = new();
        _purchasedItems = new();
        _itemNotListed = null;
        _itemsToBuy = new();
    }

    public List<ItemInput> GetPurchasedInputItems() => _purchasedInputItems;

    public void SaveNewInputItems(List<ItemInput> inputItems)
    {
        inputItems.ForEach(item => _purchasedInputItems.Add(item));
    }
    
    public List<string> GetItemsToBuy() => _itemsToBuy;

    public void UpdateListOfItemsToBuy(string item)
    {
        _itemsToBuy.Remove(item);
    }
    
    public ItemInput? GetItemNotListed() => _itemNotListed;

    public void ReserveItemNotListed(ItemInput item)
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