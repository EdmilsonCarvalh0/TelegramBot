using TelegramBot.Application;
using TelegramBot.DataModels.Item.Snapshot;
using TelegramBot.Domain.Item.Input;
using TelegramBot.Infrastructure.Json.JsonStorage;
using TelegramBot.Service.ShoppingAssistant.Utils;

namespace TelegramBot.Service.ShoppingAssistant;

public class ShoppingAssistantMode
{
    private readonly IStagingArea _stagingArea;
    private readonly ShoppingHistory _shoppingHistory;
    private readonly ShoppingListSnapshot _shoppingListSnapshot;

    public ShoppingAssistantMode(IStagingArea stagingArea, ShoppingHistory shoppingHistory)
    {
        _stagingArea = stagingArea;
        _shoppingHistory = shoppingHistory;
        _shoppingListSnapshot = new();
    }

    public void LoadList(List<string> items)
    {
        _stagingArea.SaveItemsToBuy(items);
    }

    public void BeginShoppingSession()
    {
        _shoppingListSnapshot.InitializeFromCurrentTime();
    }

    public void PrepareForCompletion()
    {
        var inputItems = _stagingArea.GetPurchasedInputItems();
        
        foreach (var inputItem in inputItems)
        {
            var item = ItemFactory.Create(
                _shoppingListSnapshot.ItemCollection.Values.Count,
                inputItem.Name,
                inputItem.Brand,
                inputItem.Price);
            
            _shoppingListSnapshot.ItemCollection.Values.Add(item);
        }
    }

    public bool RemoveItemFromShoppingList(ItemInput item)
    {
        var itemsToBuy = _stagingArea.GetItemsToBuy();
        var result = itemsToBuy.FirstOrDefault(remainingItem => 
            remainingItem.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase));

        if (result == null)  return false;

        _stagingArea.UpdateListOfItemsToBuy(result);
        _shoppingListSnapshot.IncreaseTotal(item.Price);
        return true;
    }

    public decimal GetTotalPrice()
    {
        return _shoppingListSnapshot.Total;
    }

    public List<string> GetListOfRemainingItems()
    {
        return _stagingArea.GetItemsToBuy();
    }
    
    public ItemInput GetItemReserved()
    {
        return _stagingArea.GetItemNotListed()!;
    }

    public bool CheckIfListIsEmpty()
    {
        var items = _stagingArea.GetItemsToBuy();
        return items.Count == 0;
    }

    public void ReserveNewItem(ItemInput itemNotListed)
    {
        _stagingArea.ReserveItemNotListed(itemNotListed);
    }

    public void SaveNewItem()
    {
        _stagingArea.SaveItemNotListed();
    }

    public void AddInputItemToPurchasedList(List<ItemInput> inputItems)
    {
        _stagingArea.SaveNewInputItems(inputItems);
    }

    public void FinalizeAndSaveList()
    {
        _shoppingHistory.SavePurchasedItems(_shoppingListSnapshot);
    }
}