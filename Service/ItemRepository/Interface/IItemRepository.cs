using TelegramBot.Application.Handlers;
using TelegramBot.DataModels.Item.Snapshot;
using TelegramBot.Domain.Item.Input;
using TelegramBot.Domain.ItemEntity;

namespace TelegramBot.Service.ItemRepository.Interface;

public interface IItemRepository
{
    List<ShoppingDateTime> GetAllTheDates();
    SearchResult GetItemInRepository(string itemInput);
    ItemList GetListOfItems(ShoppingDateTime shoppingDateTime);
    void UpdateItemInList(string newAttribute);
    void AddItemInEditingArea(string itemToBeChanged);
    void AddAttributeToBeChangedInEditingArea(string attributeToBeChanged);
    InteractionState VerifyNumberReferencingItem(int referenceNumber, string operation);
    void AddItemInList(List<ItemInput> inputItems);
    SearchResult RemoveItemFromList(string item);
    void CreateNewList(string items);
}