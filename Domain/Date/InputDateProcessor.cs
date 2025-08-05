using System.Text.RegularExpressions;
using TelegramBot.Domain.Interfaces;

namespace TelegramBot.Domain.Date;

public class InputDateProcessor : IInputProcessor<InputDate>
{
    private readonly string _input;
    
    public InputDateProcessor(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Informe a data corretamente.");
        }
        _input = value;
    }
    
    public InputDate ProcessInput()
    {
        var processedDate = Regex.Split(_input.Trim(), @"\s*[-,;:\/\\&#!\n\r]\s*")
            .Select(attr => attr.Trim())
            .ToList();

        return new InputDate(processedDate);
    }
}