
using TelegramBot.Data;

namespace TelegramBot.Service;

public interface IEditingArea
{
    Item Update(string newAttribute);
    void SetAttributeToBeChanged(string attribute);
    void SetItemToBeChanged(Item item);
}