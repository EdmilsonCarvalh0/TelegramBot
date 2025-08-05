namespace TelegramBot.Domain.Interfaces;

public interface IInputProcessor<out T>
{
    T ProcessInput();
}