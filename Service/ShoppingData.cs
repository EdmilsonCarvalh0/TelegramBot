using Microsoft.AspNetCore.Http.Features;
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
        return JsonConvert.DeserializeObject<DataFormatter>(File.ReadAllText(JsonFilePath))!;
    }

    public void SaveData()
    {
        File.WriteAllText(JsonFilePath, JsonConvert.SerializeObject(ListData, Formatting.Indented));
    }

    public string GetList()
    {
        string list = string.Empty;

        foreach (var item in ListData.Items)
        {
            var precoFormatado = item.Preco.ToString("C");
            list += $"{item.Nome} - {item.Marca} - {precoFormatado}\n";
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
        List<Item> temporaryList = ListData.Items;

        var itemsForAdd = new List<string>();

        itemsForAdd.AddRange(userItem.Split(" - ").ToList());
        temporaryList.Add(new Item {
            Nome = itemsForAdd[0],
            Marca = itemsForAdd[1],
            Preco = FormatPrice(itemsForAdd[2])
        });

        ListData.Items = temporaryList;
        SaveData();
    }

    public void RemoveItemFromList(string userItem)
    {
        List<Item> temporaryList = ListData.Items;

        var itemsForRemove = new List<string>();

        itemsForRemove.AddRange(userItem.Split(", ", StringSplitOptions.None).ToList());
        temporaryList.RemoveAll(item => itemsForRemove.Contains(item.Nome));

        ListData.Items = temporaryList;
        SaveData();
    }

    public void CreateNewList(string userItems)
    {

    }

    public static decimal FormatPrice(string preco)
    {
        return Convert.ToDecimal(preco);
    }
}