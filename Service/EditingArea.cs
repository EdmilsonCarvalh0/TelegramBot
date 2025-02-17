using TelegramBot.Data;

namespace TelegramBot.Service;

public class EditingArea
{
    public Item ItemToBeChanged { get; set; } = new();
    public string AttributeToBeChanged { get; set; } = string.Empty;
    private readonly Dictionary<string, Action<string>> Scenario;

    public EditingArea()
    {
        Scenario = new Dictionary<string, Action<string>>{
            {"Nome", ItemToBeChanged.SetNome},
            {"Marca", ItemToBeChanged.SetMarca},
            {"Preço", ItemToBeChanged.SetPreco}
        };
    }

    public Item Update(string newAttribute)
    {
        if(Scenario.TryGetValue(AttributeToBeChanged, out var action))
        {
            action(newAttribute);
        }

        return ItemToBeChanged;
    }
}