using TelegramBot.Domain.Interfaces;

namespace TelegramBot.Domain.Date;

public class InputDateService : IInputService<InputDate>
{
    public static InputDate ProcessRawInput(string rawInput)
    {
        var processor = new InputDateProcessor(rawInput);
        return processor.ProcessInput();
    }
}