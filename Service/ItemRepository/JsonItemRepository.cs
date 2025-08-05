using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Application;
using TelegramBot.Application.Handlers;
using TelegramBot.DataModels.Item.Snapshot;
using TelegramBot.Domain;
using TelegramBot.Domain.Item.Input;
using TelegramBot.Domain.ItemEntity;
using TelegramBot.Infrastructure.Json.JsonStorage;
using TelegramBot.Service.ItemRepository.Interface;
using TelegramBot.Service.ItemRepository.Utils;

namespace TelegramBot.Service.ItemRepository;

public class JsonItemRepository : IItemRepository
{
    private readonly ItemList _listData;
    private readonly ShoppingHistory _shoppingHistory;
    private readonly SearchResultHandler _searchResultHandler;
    private readonly IEditingArea _editingArea;
    private readonly IServiceProvider _serviceProvider;

    public JsonItemRepository(ShoppingHistory shoppingHistory, SearchResultHandler searchResultHandler, IServiceProvider serviceProvider)
    {
        _shoppingHistory = shoppingHistory;
        _listData = LoadData();
        _serviceProvider = serviceProvider;
        _editingArea = GetEditingArea();
        _searchResultHandler = searchResultHandler;
    }

    private ItemList LoadData()
    {
        var snapshot = _shoppingHistory.GetByMonth(new ShoppingDateTime("abril", 17, "09:32"));
        return snapshot!.ItemCollection;
    }

    private void SaveData()
    {
        var snapshot = _shoppingHistory.GetByMonth(new ShoppingDateTime("abril", 17, "09:32"));
        snapshot!.ItemCollection = _listData;
        _shoppingHistory.SavingChangedSnapshot(snapshot);
    }

    public List<ShoppingDateTime> GetAllTheDates()
    {
        return _shoppingHistory.GetAllTheDatesFromTheExistingLists();
    }

    private IEditingArea GetEditingArea()
    {
        using var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IEditingArea>();
    }

    public SearchResult GetItemInRepository(string itemInput)
    {       
        var result = _listData.Values.FindAll(
            item => item.Nome.Value.Equals(itemInput, StringComparison.CurrentCultureIgnoreCase));
        
        var searchResult = _searchResultHandler.GetSearchResult(result);
        
        if (searchResult.Status == SearchStatus.MoreThanOne) _editingArea.InsertFoundItems(result);

        return searchResult;
    }

    public ItemList GetListOfItems(ShoppingDateTime shoppingDateTime)
    {
        // TODO: Selecionar o objeto correto pelos atributos do ShoppingDateTime por parametrização \/
        var snapshot = _shoppingHistory.GetByMonth(shoppingDateTime);
        return _shoppingHistory.GetByMonth(shoppingDateTime)!.ItemCollection;
        // return .ItemDataFormatter.Items
        //     .Select(item => 
        //     ItemFactory.Create(
        //         item.Id.Value, 
        //         item.Nome.Value, 
        //         item.Marca.Value, 
        //         item.Preco.Value)
        //     ).ToList();
    }

    public void UpdateItemInList(string newAttribute)
    {
        var itemUpdated = _editingArea.Update(newAttribute);
        var indexItem = _listData.Values.FindIndex(item => item.Id.Value == itemUpdated.Id.Value);

        _listData.Values[indexItem] = itemUpdated;

        SaveData();
    }

    public void AddItemInEditingArea(string itemToBeChanged)
    {
        var item = _listData.GetItemInList(itemToBeChanged);

        if (item == null) return;
        var it = ItemFactory.Create(
            item.Id.Value,
            item.Nome.Value,
            item.Marca.Value,
            item.Preco.Value
        );

        _editingArea.SetItemToBeChanged(it);
    }

    public void AddAttributeToBeChangedInEditingArea(string attributeToBeChanged)
    {
        _editingArea.SetAttributeToBeChanged(attributeToBeChanged);
    }

    public void AddItemInList(List<ItemInput> inputItems)
    {
        foreach (var input in inputItems)
        {
            var item = ItemFactory.Create(
                _listData.Values.Count,
                input.Name,
                input.Brand,
                input.Price
            );
            _listData.Values.Add(item);
        }
        SaveData();
    }

    public InteractionState VerifyNumberReferencingItem(int referenceNumber, string operation)
    {
        switch (operation)
        {
            case "waiting_for_number_that_references_to_update":
            {
                var item = _searchResultHandler.GetItemToUpdate(referenceNumber)!;

                if (item == null) return InteractionState.None;

                _editingArea.SetItemToBeChanged(ItemFactory.Create(
                    item.Id.Value,
                    item.Nome.Value,
                    item.Marca.Value,
                    item.Preco.Value
                ));

                return InteractionState.UpdateItem;
            }
            case "waiting_for_number_that_references_to_remove":
            {
                var item = _searchResultHandler.GetItemToUpdate(referenceNumber)!;

                if (item == null) return InteractionState.None;

                _listData.Values.Remove(item);

                _listData.SequentializeIDs();
                SaveData();
                break;
            }
        }

        return InteractionState.DeleteItem;
    }

    public SearchResult RemoveItemFromList(string userItem)
    {
        //TODO: Verify implementation of SearchResultHandler and to refactor logic
        var result = _listData.Values.FindAll(
            it => it.Nome.Value.Equals(userItem, StringComparison.CurrentCultureIgnoreCase));

        var searchResult = _searchResultHandler.GetSearchResult(result);

        if (searchResult.Status is SearchStatus.NotFound or SearchStatus.MoreThanOne) return searchResult;
        
        _listData.Values.Remove(result[0]);

        _listData.SequentializeIDs();
        SaveData();

        return searchResult;
    }

    public void CreateNewList(string userItems)
    {

    }
}