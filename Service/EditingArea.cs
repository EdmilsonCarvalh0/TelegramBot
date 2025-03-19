using TelegramBot.Data;

namespace TelegramBot.Service;

public class EditingArea : IEditingArea
{
    public Item ItemToBeChanged { get; set; } = new();
    public string AttributeToBeChanged { get; set; } = string.Empty;
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
}