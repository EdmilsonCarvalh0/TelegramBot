using System.Text.RegularExpressions;
using TelegramBot.Domain.Interfaces;

namespace TelegramBot.Domain.Item.Input;

public class ItemInputProcessor : IInputProcessor<List<ItemInput>>
{
    private readonly string _input;
    private readonly List<ItemInput> _inputItemsValid = new();
    
    public ItemInputProcessor(string value)
    {
        if (string.IsNullOrEmpty((value)) || (value.Length <= 2))
        {
            throw new ArgumentException("Informe o item corretamente.");
        }
        _input = value;
    }
    
    public List<ItemInput> ProcessInput()
    {
        var linesWithUnprocessedItems = _input
            .Trim()
            .Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        
        PrepareInput(linesWithUnprocessedItems);
        return _inputItemsValid;
    }
    
    private void PrepareInput(List<string> linesWithUnprocessedItems)
    {
        foreach (var line in linesWithUnprocessedItems)
        {
            var itemAttributes = Regex.Split(line.Trim(), @"\s*[-,;:|/\\&#!]\s*")
                                                    .Select(attr => attr.Trim())
                                                    .ToList();
            
            ItemInput inputItemValid = new(itemAttributes);
            _inputItemsValid.Add(inputItemValid);
        }
    }
}