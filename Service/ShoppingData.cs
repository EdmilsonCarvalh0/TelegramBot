using Newtonsoft.Json;

namespace TelegramBot.Data;

public class ShoppingData : IShoppingData
{
    public DataFormatter ListOfItems = new();
    public string JsonFilePath = "C:/Users/ANGELA SOUZA/OneDrive/Área de Trabalho/ED/Programação/C#/Projects/TelegramBot/Data/data.json";

    public void LoadData()
    {
        string json = File.ReadAllText(JsonFilePath);
        DataFormatter ListOfItems = JsonConvert.DeserializeObject<DataFormatter>(json)!;
    }

    public string GetList()
    {
        DataFormatter formatter = new DataFormatter{
            Items = new List<Item>
            {
                new Item { Nome = "Pão", Marca = "Bauducco", Preco = 4.9m },
                new Item { Nome = "Queijo", Marca = "Piracanjuba", Preco = 8.3m }
            }
        };
        
        return JsonConvert.SerializeObject(formatter, Formatting.Indented);
        //TODO: implement json deserialize for return list in method
        //      implementar deserialização json para retornar a lista no método
    }

    public void UpdateList(string items)
    {
        //TODO: implement list update and correctly replace elements
        //      implementar atualização de lista e substituir corretamente os elementos
    }

    public void AddItemInList(string item)
    {
        //TODO: implement the serialization to add in list
        //      implemente a serialização para adicionar na lista
    }

    public void RemoveItemFromList(string item)
    {
        
    }

    public void CreateNewList(string items)
    {

    }
}