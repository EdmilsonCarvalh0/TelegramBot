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
            BotClient.SetInputMessage(message.Text!);

            var startService = BotClient.StartService();
            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Olá, bem vindo ao Bot de Compras Mensais!\nEscolha uma opção:",
                replyMarkup: startService,
                cancellationToken: cancellationToken
            );

            Console.WriteLine($"{message.From?.Username} saying: {message.Text}");
        }
        else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery is not null)
        {
            var callbackQuery = update.CallbackQuery;
            var originalMessage = callbackQuery.Data;

            await botClient.AnswerCallbackQuery(
                callbackQueryId: callbackQuery.Id,
                text: $"Opção selecionada: {callbackQuery.Data}",
                cancellationToken: cancellationToken
            );
            Console.WriteLine($"Opção selecionada: {callbackQuery.Data}");

            string response = BotClient.ExecuteChosenOption(originalMessage!);
            
            await botClient.SendMessage(
                chatId: callbackQuery.Message!.Chat.Id,
                text: response,
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