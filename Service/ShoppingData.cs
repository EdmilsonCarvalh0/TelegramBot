using System.Data.Common;
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
        List<string> itemsForAdd = new();

        //TODO: refactor to create function
        if (userItem.Contains('\n'))
        {
            List<string> linesWithItems =
            [
                .. userItem.Trim().Split("\n"),
            ];

            // linesWithItems.ForEach(x => itemsForAdd.AddRange(x.Trim().Split(" - ")));
            foreach (var line in linesWithItems)
            {
                itemsForAdd.Clear();
                itemsForAdd.AddRange(line.Trim().Split(" - "));
                ListData.Items.Add(CheckLineOfItem(itemsForAdd));
            }
            
            // ListData.Items.Add(CheckLineOfItem());
        }

        itemsForAdd.AddRange(userItem.Trim().Split(" - "));

        ListData.Items.Add(new Item {
            Nome = itemsForAdd[0],
            Marca = itemsForAdd[1],
            Preco = FormatPrice(itemsForAdd[2])
        });

        SaveData();
    }

    public void RemoveItemFromList(string userItem)
    {
        List<Item> temporaryList = ListData.Items;

        var itemsForRemove = new List<string>();

        itemsForRemove.AddRange(userItem.Trim()
                                        .Split(", ", StringSplitOptions.None)
                                        .ToList());
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

    public static Item CheckLineOfItem(List<string> linesOfItems)
    {
        var indefinedItem = linesOfItems[1].Split(",").ToList();
        bool isPrice = indefinedItem[0].All(char.IsDigit) && indefinedItem[1].All(char.IsDigit);
        
        return isPrice ? 
            new Item {
                Nome = linesOfItems[0],
                Marca = default!,
                Preco = FormatPrice(linesOfItems[1])
            } : 
            new Item 
            {
                Nome = linesOfItems[0],
                Marca = linesOfItems[1],
                Preco = FormatPrice(linesOfItems[2])
            };
    }
}