using TelegramBot.Domain.Item.Input;

namespace TelegramBot.Service.ShoppingAssistant.Utils;

public interface IStagingArea
{
    List<InputItem> GetPurchasedInputItems();
    void SaveNewInputItems(List<InputItem> inputItems);
    List<string> GetItemsToBuy();
    void UpdateListOfItemsToBuy(string item);
    InputItem? GetItemNotListed();
    void ReserveItemNotListed(InputItem item);
    void SaveItemNotListed();
    void SaveItemsToBuy(List<string> items);
}