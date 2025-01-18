using TelegramBot.Service;

public interface IItemRepository
{
    string GetItemInRepository(string itemInput);
    string GetList();
    void UpdateList(string items);
    void AddItemInList(string items);
    void RemoveItemFromList(string item);
    void CreateNewList(string items);
}