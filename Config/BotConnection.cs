using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Entities;

public class BotConnection
{
    public BotConnection()
    {
        
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message is not null)
        {
            var message = update.Message;
            BotClient.GetInputMessage(message.Text!);

            Console.WriteLine($"{message.From?.Username} saying: {message.Text}");

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"You saying: {message.Text}",
                cancellationToken: cancellationToken
            );
        }
    }

    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Erro recebido: {exception.Message}");
        return Task.CompletedTask;
    }
}