using Newtonsoft.Json;
using TelegramBot.Data;

namespace TelegramBot.Service;

public class JsonItemRepository : IItemRepository
{
    public DataFormatter ListData = new();
    public string JsonFilePath = "C:/Users/ANGELA SOUZA/OneDrive/Área de Trabalho/ED/Programação/C#/Projects/TelegramBot/Data/data.json";

    public JsonItemRepository()
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

    public string GetItemInRepository(string itemInput)
    {       
        var result = ListData.Items.FindAll(
            delegate (Item it)
            {
                return it.Nome.Equals(itemInput, StringComparison.CurrentCultureIgnoreCase);
            }
        );

        if (result.Count == 0) return "Item não encontrado.";

        string names = string.Empty;

        if (result.Count > 1)
        {
            foreach (var item in result)
            {
                var precoFormatado = item.Preco.ToString("C");
                names += $"{item.Nome} - {item.Marca} - {precoFormatado}\n";
            }

            return names;
        }

        return names;
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
            Nome = itemsToAdd[0],
            Marca = itemsToAdd[1],
            Preco = FormatPrice(itemsToAdd[2])
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

    private static Item CheckLineOfItem(List<string> linesOfItems)
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