namespace TelegramBot.Data;

public class ShoppingData : IShoppingData
{
    public string GetList()
    {
        return "Lista vazia";
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