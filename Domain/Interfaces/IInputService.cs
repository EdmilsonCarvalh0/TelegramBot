namespace TelegramBot.Domain.Interfaces;

public interface IInputService<out T>
{
    static abstract T ProcessRawInput(string rawInput);
}