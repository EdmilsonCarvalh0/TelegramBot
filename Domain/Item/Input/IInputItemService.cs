namespace TelegramBot.Domain.Item;

public interface IInputItemService
{
    List<InputItem> ProcessRawInput(string rawInput);
}