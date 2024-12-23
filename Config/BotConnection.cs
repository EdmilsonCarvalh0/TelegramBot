using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Entities;

public class BotConnection
{
    private static readonly ConcurrentDictionary<long, string> UserStates = new();
    private static readonly Dictionary<string, string> States;
    static BotConnection()
    {
        //TODO: check if the states are accessible in method
        States = new Dictionary<string, string>
        {
            {"Option 01", "waiting_for_item_to_update_list"},
            {"Option 02", "waiting_item"},
            {"Option 03", "waiting_items_to_create_new_list"}
        };
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message is not null)
        {
            var message = update.Message;
            BotClient.SetInputMessage(message.Text!);

            long userId = message.From!.Id;

            //Verify user status to send item for list atualization
            if (UserStates.TryGetValue(userId, out var states) && states == "waiting_for_item_to_update_list")
            {
                var item = message.Text;

                string methodResponse = BotClient.SendItemToUpdateList(item!);

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: methodResponse,
                    cancellationToken: cancellationToken
                );

                UserStates.TryRemove(userId, out _);
            }
            
            //Verify user status for item insertion
            if (UserStates.TryGetValue(userId, out var stateOfWaitingItem) && stateOfWaitingItem == "waiting_item")
            {
                var item = message.Text;

                string methodResponse = BotClient.AddItemInShoppingData(item!);
                
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: methodResponse,
                    cancellationToken: cancellationToken
                );

                UserStates.TryRemove(userId, out _);
            }

            //Verify user status to create new list with item
            if (UserStates.TryGetValue(userId, out var state) && state == "waiting_items_to_create_new_list")
            {
                var item = message.Text;
                
                string methodResponse = BotClient.GetItemsToCreatelist(item!);
                 
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: methodResponse,
                    cancellationToken: cancellationToken
                );

                UserStates.TryRemove(userId, out _);
            }

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
            long userId = callbackQuery.From.Id;

            //Verify option 'ver lista' and send list to user
            if (callbackQuery.Data == "Ver lista")
            {
                await botClient.AnswerCallbackQuery(
                    callbackQueryId: callbackQuery.Id,
                    text: "Enviando lista...",
                    cancellationToken: cancellationToken
                );

                var methodResponse = BotClient.ShowList();
                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: methodResponse,
                    cancellationToken: cancellationToken
                );
            }

            //Verify option 'adicionar item' and asks the user
            if (callbackQuery.Data == "Adicionar item")
            {
                UserStates[userId] = "waiting_item";

                await botClient.AnswerCallbackQuery(
                    callbackQueryId: callbackQuery.Id,
                    text: $"Digite o nome do item que deseja adicionar.",
                    cancellationToken: cancellationToken
                );
                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: "Por favor, envie o nome agora:",
                    cancellationToken: cancellationToken
                );
            }

            //Verify option 'atualizar lista' and asks the user wich kind atualization
            if (callbackQuery.Data == "Atualizar lista")
            {
                UserStates[userId] = "waiting_for_item_to_update_list";

                await botClient.AnswerCallbackQuery(
                    callbackQueryId: callbackQuery.Id,
                    text: "Selecione o tipo da atualização.",
                    cancellationToken: cancellationToken
                );

                var listUpdatesOptionsKeyboard = BotClient.GetOptionsOfListUpdate();
                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: "Tipos de atualizações disponíveis:",
                    replyMarkup: listUpdatesOptionsKeyboard,
                    cancellationToken: cancellationToken
                );
            }

            //Verify option 'criar nova lista' 
            if (callbackQuery.Data == "Criar nova lista")
            {
                UserStates[userId] = "waiting_items_to_create_new_list";

                await botClient.AnswerCallbackQuery(
                    callbackQueryId: callbackQuery.Id,
                    text: "Informe os itens da nova lista.",
                    cancellationToken: cancellationToken
                );
                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: "Envie agora os itens da nova lista.\nUse o padrão adequado para registro correto dos itens. Nesse caso, separe os itens por vírgula sem espaços.\nEx: Pão,Manteiga,Queijo",
                    cancellationToken: cancellationToken
                );
            }

            Console.WriteLine($"Opção selecionada: {callbackQuery.Data}");
        }
    }

    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Erro recebido: {exception.Message}");
        return Task.CompletedTask;
    }
}