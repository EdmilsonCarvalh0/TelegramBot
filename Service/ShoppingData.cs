using Newtonsoft.Json;

namespace TelegramBot.Data;

public class ShoppingData : IShoppingData
{
    public DataFormatter ListData = new();
    public string JsonFilePath = "C:/Users/ANGELA SOUZA/OneDrive/Área de Trabalho/ED/Programação/C#/Projects/TelegramBot/Data/data.json";

    public ShoppingData()
    {
        ListData = LoadData();
    }

    public DataFormatter LoadData()
    {
        string json = File.ReadAllText(JsonFilePath);
        return JsonConvert.DeserializeObject<DataFormatter>(json)!;
    }

    public string GetList()
    {
        string list = string.Empty;

        foreach (var item in ListData.Items)
        {
            list += $"{item.Nome} - {item.Marca} - {item.Preco}\n";
        }

        return list;
    }

    public void UpdateList(string userItems)
    {
        //TODO: implement list update and correctly replace elements
        //      implementar atualização de lista e substituir corretamente os elementos
    }

    public void AddItemInList(string userItem)
    {
        //TODO: implement the serialization to add in list
        //      implemente a serialização para adicionar na lista
    }

    public void RemoveItemFromList(string userItem)
    {
        List<Item> temporaryList = ListData.Items;

        var itemsForRemove = new List<string>();

        itemsForRemove.AddRange(userItem.Split(", ", StringSplitOptions.None).ToList());
        temporaryList.RemoveAll(item => itemsForRemove.Contains(item.Nome));

        ListData.Items = temporaryList;
    }

    public void CreateNewList(string userItems)
    {

    }
}