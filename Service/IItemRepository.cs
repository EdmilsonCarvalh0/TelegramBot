using TelegramBot.Domain;
using TelegramBot.Service;

public interface IItemRepository
{
    SearchResultDTO GetItemInRepository(string itemInput);
    string GetList();
    void UpdateList(string items);
    void UpdateItemInList(string newAttribute);
    void AddItemInEditingArea(string itemToBeChanged);
    void AddAttributeToBeChangedInEditingArea(string attributeToBeChanged);
    void AddItemInList(string items);
    SearchResultDTO RemoveItemFromList(string item);
    void CreateNewList(string items);
}