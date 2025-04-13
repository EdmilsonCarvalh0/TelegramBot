using Domain.Item;

namespace TelegramBot.Service;

public class ShoppingAssistantMode
{
    private List<Item> PurchasedItems { get; set; } = new();
    private List<string> ItemsToBuy = new();
    private string ItemNotListed = string.Empty;

    public void LoadList(List<string> items)
    {
        ItemsToBuy = items;
    }

    public bool RemoveItemFromShoppingList(string item)
    {
        var result = ItemsToBuy.FirstOrDefault(remainingItem => remainingItem.Equals(item, StringComparison.CurrentCultureIgnoreCase));

        if (result == null)  return false;

        ItemsToBuy.Remove(result);
        return true;
    }

    public List<string> GetListOfRemainingItems()
    {
        return ItemsToBuy;
    }

    public bool ChekIfListIsEmpty()
    {
        return ItemsToBuy.Count == 0;
    }

    public void ReserveItemNotListed(string itemNotListed)
    {
        ItemNotListed = itemNotListed;
    }

    public string GetItemReserved()
    {
        return ItemNotListed;
    }
}