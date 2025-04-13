using System.Globalization;

namespace TelegramBot.Domain.Item;

public class InputProcessor
{
    private readonly string _input;
    private readonly List<InputItem> _inputItemsValid = new();
    
    public InputProcessor(string value)
    {
        if (string.IsNullOrEmpty((value)) || (value.Length <= 2))
        {
            throw new ArgumentException("Informe o item corretamente.");
        }
        _input = value;
    }
    
    public List<InputItem> ProcessInput()
    {
        var linesWithUnprocessedItems = _input
            .Trim()
            .Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        
        PrepareInputItems(linesWithUnprocessedItems);
        return _inputItemsValid;
    }

    private void PrepareInputItems(List<string> linesWithUnprocessedItems)
    {
        foreach (var line in linesWithUnprocessedItems)
        {
            var itemAttributes = line
                .Trim()
                .Split(" - ")
                .ToList();
            
            InputItem inputItemValid = new(itemAttributes);
            _inputItemsValid.Add(inputItemValid);
        }
    }
}