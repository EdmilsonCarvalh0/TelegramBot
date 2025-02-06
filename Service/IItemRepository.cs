using TelegramBot.Domain;

public interface IItemRepository
{
    ItemSearchResult GetItemInRepository(string itemInput);
    string GetList();
    void UpdateList(string items);
    void AddItemInList(string items);
    void RemoveItemFromList(string item);
    void CreateNewList(string items);
}