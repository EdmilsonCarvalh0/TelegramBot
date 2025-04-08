using Domain.Item;

namespace TelegramBot.Service;

public class EditingArea : IEditingArea
{
    private Item ItemToBeChanged { get; set; } = Item.Placeholder;
    private string AttributeToBeChanged { get; set; } = string.Empty;
    private List<Item> ItemsFound { get; set; } = [];
    private Dictionary<string, Action<string>> Scenario 
    {
        get 
        {
            return new Dictionary<string, Action<string>>
            {
                {"Nome", ItemToBeChanged.SetNome},
                {"Marca", ItemToBeChanged.SetMarca},
                {"Preço", ItemToBeChanged.SetPreco}
            };
        }
    }

    public Item Update(string newAttribute)
    {
        if(Scenario.TryGetValue(AttributeToBeChanged, out var action))
        {
            action(newAttribute);
        }

        return ItemToBeChanged;
    }

    public void SetAttributeToBeChanged(string attribute)
    {
        AttributeToBeChanged = attribute;
    }

    public void SetItemToBeChanged(Item item)
    {
        ItemToBeChanged = item;
    }

    public void InsertFoundItems(List<Item> itemsFound)
    {
        ItemsFound = itemsFound;
    }
}