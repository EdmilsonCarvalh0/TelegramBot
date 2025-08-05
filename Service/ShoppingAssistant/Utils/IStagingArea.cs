using TelegramBot.Domain.Item.Input;

namespace TelegramBot.Service.ShoppingAssistant.Utils;

// AVALIAR INTERFACE ADEQUADA PARA TODAS AS STAGING AREAS
public interface IStagingArea
{
    List<ItemInput> GetPurchasedInputItems();
    void SaveNewInputItems(List<ItemInput> inputItems);
    List<string> GetItemsToBuy();
    void UpdateListOfItemsToBuy(string item);
    ItemInput? GetItemNotListed();
    void ReserveItemNotListed(ItemInput item);
    void SaveItemNotListed();
    void SaveItemsToBuy(List<string> items);
}