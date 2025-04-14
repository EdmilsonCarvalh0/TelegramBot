using Domain.Item;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TelegramBot.Application;
using TelegramBot.Domain;
using TelegramBot.Domain.Item;

namespace TelegramBot.Service;

public class JsonItemRepository : IItemRepository
{
    private readonly ItemDataFormatter _listData;
    private readonly string _jsonFilePath;
    private readonly SearchResultHandler _searchResultHandler;
    private readonly IEditingArea _editingArea;
    private readonly IServiceProvider _serviceProvider;

    public JsonItemRepository(SearchResultHandler searchResultHandler, IServiceProvider serviceProvider, string filePath)
    {
        _jsonFilePath = filePath;
        _listData = LoadData();
        _serviceProvider = serviceProvider;
        _editingArea = GetEditingArea();
        _searchResultHandler = searchResultHandler;
    }

    private ItemDataFormatter LoadData()
    {
        return JsonConvert.DeserializeObject<ItemDataFormatter>(File.ReadAllText(_jsonFilePath))!;
    }

    private void SaveData()
    {
        File.WriteAllText(_jsonFilePath, JsonConvert.SerializeObject(_listData, Formatting.Indented));
    }

    private IEditingArea GetEditingArea()
    {
        using var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IEditingArea>();
    }

    public SearchResult GetItemInRepository(string itemInput)
    {       
        var result = _listData.Items.FindAll(
            item => item.Nome.Value.Equals(itemInput, StringComparison.CurrentCultureIgnoreCase));
        
        var searchResult = _searchResultHandler.GetSearchResult(result);
        
        if (searchResult.Status == SearchStatus.MoreThanOne) _editingArea.InsertFoundItems(result);

        return searchResult;
    }

    public List<Item> GetListOfItems()
    {
        return _listData.Items.Select(item => 
            ItemFactory.Create(
                item.Id.Value, 
                item.Nome.Value, 
                item.Marca.Value, 
                item.Preco.Value)
            ).ToList();
    }

    public void UpdateItemInList(string newAttribute)
    {
        var itemUpdated = _editingArea.Update(newAttribute);
        var indexItem = _listData.Items.FindIndex(item => item.Id.Value == itemUpdated.Id.Value);

        _listData.Items[indexItem] = itemUpdated;

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

    public void AddItemInList(List<InputItem> inputItems)
    {
        foreach (var input in inputItems)
        {
            var item = ItemFactory.Create(
                _listData.Items.Count,
                input.Name,
                input.Brand,
                input.Price
            );
            _listData.Items.Add(item);
        }
        SaveData();
    }

    public UserState VerifyNumberReferencingItem(int referenceNumber, string operation)
    {
        switch (operation)
        {
            case "waiting_for_number_that_references_to_update":
            {
                var item = _searchResultHandler.GetItemToUpdate(referenceNumber)!;

                if (item == null) return UserState.None;

                _editingArea.SetItemToBeChanged(ItemFactory.Create(
                    item.Id.Value,
                    item.Nome.Value,
                    item.Marca.Value,
                    item.Preco.Value
                ));

                return UserState.UpdateItem;
            }
            case "waiting_for_number_that_references_to_remove":
            {
                var item = _searchResultHandler.GetItemToUpdate(referenceNumber)!;

                if (item == null) return UserState.None;

                _listData.Items.Remove(item);

                _listData.SequentializeIDs();
                SaveData();
                break;
            }
        }

        return UserState.DeleteItem;
    }

    public SearchResult RemoveItemFromList(string userItem)
    {
        //TODO: Verify implementation of SearchResultHandler and to refactor logic
        var result = _listData.Items.FindAll(
            it => it.Nome.Value.Equals(userItem, StringComparison.CurrentCultureIgnoreCase));

        var searchResult = _searchResultHandler.GetSearchResult(result);

        if (searchResult.Status is SearchStatus.NotFound or SearchStatus.MoreThanOne) return searchResult;
        
        _listData.Items.Remove(result[0]);

        _listData.SequentializeIDs();
        SaveData();

        return searchResult;
    }

    public void CreateNewList(string userItems)
    {

    }
}