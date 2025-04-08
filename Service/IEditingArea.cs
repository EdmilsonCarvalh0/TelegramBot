
using Domain.Item;

namespace TelegramBot.Service;

public interface IEditingArea
{
    Item Update(string newAttribute);
    void SetAttributeToBeChanged(string attribute);
    void SetItemToBeChanged(Item item);
    void InsertFoundItems(List<Item> itemsFound);
}