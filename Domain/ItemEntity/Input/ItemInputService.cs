using TelegramBot.Domain.Interfaces;

namespace TelegramBot.Domain.Item.Input;

public class ItemInputService : IInputService<List<ItemInput>>
{
    public static List<ItemInput> ProcessRawInput(string rawInput)
    {
        var processor = new ItemInputProcessor(rawInput);
        return processor.ProcessInput();
    }
}