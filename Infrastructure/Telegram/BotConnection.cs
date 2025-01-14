using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Core;
using TelegramBot.Infrastructure.Handlers;

public class BotConnection
{
    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        long userId = update.CallbackQuery?.From.Id ?? update.Message?.Chat.Id ?? 0;

        var context = new BotRequestContext(
            botClient,
            userId,
            update.CallbackQuery,
            update.Message,
            cancellationToken
        );

        var handlers = new UpdateHandlers(context);

        if (update.Type == UpdateType.Message && update.Message != null)
        {
            await handlers.HandleMessageAsync();
        }
        else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
        {
            await handlers.HandleCallbackQueryAsync();
        }
    }

    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Erro recebido: {exception.Message}\nData: {exception.Data}");
        return Task.CompletedTask;
    }

    public InlineKeyboardMarkup StartService()
    {
        throw new NotImplementedException();
    }

    public InlineKeyboardMarkup GetOptionsOfListUpdate()
    {
        throw new NotImplementedException();
    }

    public string AddItemInShoppingData(string item)
    {
        throw new NotImplementedException();
    }

    public string SendItemToUpdateList(string item)
    {
        throw new NotImplementedException();
    }

    public string SendItemToRemoveFromList(string item)
    {
        throw new NotImplementedException();
    }

    public string ShowList()
    {
        throw new NotImplementedException();
    }

    public string GetItemsToCreatelist(string item)
    {
        throw new NotImplementedException();
    }
}
