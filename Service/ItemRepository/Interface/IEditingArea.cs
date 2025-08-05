using TelegramBot.Domain.ItemEntity;

namespace TelegramBot.Service.ItemRepository.Interface;

public interface IEditingArea
{
    Item Update(string newAttribute);
    void SetAttributeToBeChanged(string attribute);
    void SetItemToBeChanged(Item item);
    void InsertFoundItems(List<Item> itemsFound);
}