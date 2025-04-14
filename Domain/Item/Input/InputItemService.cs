namespace TelegramBot.Domain.Item;

public class InputItemService : IInputItemService
{
    public List<InputItem> ProcessRawInput(string rawInput)
    {
        var processor = new InputProcessor(rawInput);
        return processor.ProcessInput();
    }
}