using Domain.Item;
using TelegramBot.Domain;
using TelegramBot.Service;

public interface IItemRepository
{
    SearchResult GetItemInRepository(string itemInput);
    List<Item> GetListOfItems();
    void UpdateItemInList(string newAttribute);
    void AddItemInEditingArea(string itemToBeChanged);
    void AddAttributeToBeChangedInEditingArea(string attributeToBeChanged);
    UserState VerifyNumberReferencingItem(int referenceNumber, string operation);
    void AddItemInList(string items);
    SearchResult RemoveItemFromList(string item);
    void CreateNewList(string items);
}