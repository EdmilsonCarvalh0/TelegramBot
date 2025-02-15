using TelegramBot.Domain;
using TelegramBot.Service;

public interface IItemRepository
{
    SearchResultDTO GetItemInRepository(string itemInput);
    string GetList();
    void UpdateList(string items);
    void AddItemInEditingArea(string itemToBeChanged);
    void AddItemInList(string items);
    SearchResultDTO RemoveItemFromList(string item);
    void CreateNewList(string items);
}