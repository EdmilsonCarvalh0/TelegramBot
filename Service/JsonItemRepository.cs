using Newtonsoft.Json;
using TelegramBot.Data;
using TelegramBot.Domain;

namespace TelegramBot.Service;

public class JsonItemRepository : IItemRepository
{
    public ItemDataFormatter ListData = new();
    private string JsonFilePath = "C:/Users/edcar/Documents/ED/Programação/C#/C#/Projects/TelegramBot/Data/ItemModel/itemsData.json";
    private SearchResultHandler _searchResultHandler { get; set; } = new();
    private readonly EditingArea _editingArea = new();

    public JsonItemRepository()
    {
        ListData = LoadData();
    }

    public ItemDataFormatter LoadData()
    {
        return JsonConvert.DeserializeObject<ItemDataFormatter>(File.ReadAllText(JsonFilePath))!;
    }

    public void SaveData()
    {
        File.WriteAllText(JsonFilePath, JsonConvert.SerializeObject(ListData, Formatting.Indented));
    }

    public SearchResultDTO GetItemInRepository(string itemInput)
    {       
        List<Item> result = ListData.Items.FindAll(
            delegate (Item it)
            {
                return it.Nome.Equals(itemInput, StringComparison.CurrentCultureIgnoreCase);
            }
        );

        return _searchResultHandler.GetSearchResult(result);
    }

    public string GetList()
    {
        string list = string.Empty;

        foreach (var item in ListData.Items)
        {
            list += item.ToString();
        }

        Console.WriteLine(list);
        return list;
    }

    public void UpdateList(string item)
    {
        //TODO: implement list update and correctly replace elements
        //      implementar atualização de lista e substituir corretamente os elementos
        /*
            In a way that it correctly changed the passed attribute.
            Perhaps the secret lies in the BotClient's automatic item verification
            
            De uma forma que ele altere corretamente o atributo passado.
            Talvez o segredo esteja na verificação automática de item do BotClient
        */


    }

    public void UpdateItemInList(string newAttribute)
    {
        var itemUpdated = _editingArea.Update(newAttribute);
        var indexItem = ListData.Items.FindIndex(item => item.Id == itemUpdated.Id);

        ListData.Items[indexItem] = itemUpdated;

        SaveData();
    }

    public void AddItemInEditingArea(string itemToBeChanged)
    {
        var item = ListData.Items.FirstOrDefault(
            item => item.Nome.Equals(itemToBeChanged, StringComparison.CurrentCultureIgnoreCase));

        if(item != null)
        {
            _editingArea.ItemToBeChanged = new Item {
                Id = item.Id,
                Nome = item.Nome,
                Marca = item.Marca,
                Preco = item.Preco
            };
        }

        //TODO: Implementar o uso da Area de Edição de item.
        // EditingArea.Add(
        //     ListData.Items.Find(
        //         delegate (Item it)
        //         {
        //             return it.Nome.Equals(itemToBeChanged, StringComparison.CurrentCultureIgnoreCase);
        //         }
        //     )
        // );
    }

    public void AddAttributeToBeChangedInEditingArea(string attributeToBeChanged)
    {
        _editingArea.AttributeToBeChanged = attributeToBeChanged;
    }

    public void AddItemInList(string userItem)
    {
        List<string> itemsToAdd = new();

        if (userItem.Contains('\n'))
        {
            AddIfThereIsMoreOneItem(userItem);
            return;
        }

        itemsToAdd.AddRange(userItem.Trim().Split(" - "));

        ListData.Items.Add(new Item {
            Id = ListData.Items.Count +1, 
            Nome = itemsToAdd[0],
            Marca = itemsToAdd[1],
            Preco = FormatPrice(itemsToAdd[2])
        });

        SaveData();
    }

    public SearchResultDTO RemoveItemFromList(string userItem)
    {
        //TODO: Verify implementation of SearchResultHandler and to refactor logic
        List<Item> result = ListData.Items.FindAll(
            delegate (Item it)
            {
                return it.Nome.Equals(userItem, StringComparison.CurrentCultureIgnoreCase);
            }
        );

        var searchResult = _searchResultHandler.GetSearchResult(result);

        if (searchResult.Status == SearchStatus.NotFound || searchResult.Status == SearchStatus.MoreThanOne) return searchResult;
        
        // List<Item> temporaryList = ListData.Items;
        // temporaryList.Remove(result[0]);
        // ListData.Items = temporaryList;
        ListData.Items.Remove(result[0]);

        SequentializeIDs();
        SaveData();

        return searchResult;
    }

    public void CreateNewList(string userItems)
    {

    }

    private static decimal FormatPrice(string preco)
    {
        return Convert.ToDecimal(preco);
    }

    private void AddIfThereIsMoreOneItem(string userItem)
    {
        List<string> linesWithItems = [.. userItem.Trim().Split('\n')];

        List<string> attributesOfTheItemToBeChecked = new();
        
        foreach (var line in linesWithItems)
        {
            attributesOfTheItemToBeChecked.Clear();
            attributesOfTheItemToBeChecked.AddRange(line.Trim().Split(" - "));
            ListData.Items.Add(CheckLineOfItem(attributesOfTheItemToBeChecked));
        }
        SaveData();
    }

    private Item CheckLineOfItem(List<string> linesOfItems)
    {
        var indefinedItem = linesOfItems[1].Split(",").ToList();
        bool isPrice = indefinedItem[0].All(char.IsDigit) && indefinedItem[1].All(char.IsDigit);
        
        return isPrice ? 
            new Item {
                Id = ListData.Items.Count +1,
                Nome = linesOfItems[0],
                Marca = default!,
                Preco = FormatPrice(linesOfItems[1])
            } : 
            new Item 
            {
                Id = ListData.Items.Count +1,
                Nome = linesOfItems[0],
                Marca = linesOfItems[1],
                Preco = FormatPrice(linesOfItems[2])
            };
    }

    private void SequentializeIDs()
    {
        if(ListData.Items.Count <= 1) return;
        
        List<Item> temporaryList = ListData.Items;
        temporaryList[0].Id = 1;

        for (int i = 0; i < temporaryList.Count -1; i++)
        {
            temporaryList[i+1].Id = temporaryList[i+1].Id - temporaryList[i].Id > 1 ? temporaryList[i].Id+1 : temporaryList[i+1].Id;
        }

        ListData.Items = temporaryList;
    }
}