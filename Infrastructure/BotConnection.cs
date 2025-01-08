using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Entities;

public class BotConnection
{
    private static readonly ConcurrentDictionary<long, string> UserStates = new();
    private static readonly Dictionary<string, string> States;
    private static readonly IBotClient _botClient = new BotClient();

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

            long userId = message.From!.Id;

            //Verify user status for item insertion
            if (UserStates.TryGetValue(userId, out var stateOfWaitingItem) && stateOfWaitingItem == "waiting_item_to_add")
            {
                var item = message.Text;

                string methodResponse = _botClient.AddItemInShoppingData(item!);
                
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: methodResponse,
                    cancellationToken: cancellationToken
                );

                UserStates.TryRemove(userId, out _);
            }

            //Verify user status to send item for list atualization
            if (UserStates.TryGetValue(userId, out var states) && states == "waiting_for_attribute_to_updated")
            {
                var attribute = message.Text;

                string methodResponse = _botClient.SendItemToUpdateList(attribute!);

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: methodResponse,
                    cancellationToken: cancellationToken
                );

                UserStates.TryRemove(userId, out _);
            }

            //implementar a resposta "waiting_for_name_attribute_to_updated" para seguir o fluxo
            if (UserStates.TryGetValue(userId, out var stateToUpdate) && stateToUpdate == "waiting_for_name_attribute_to_updated")
            {
                var attributeName = message.Text;

                var attributeOptionsKeyboard = _botClient.GetAttributeOptions();
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "O que deseja alterar?",
                    replyMarkup: attributeOptionsKeyboard,
                    cancellationToken: cancellationToken
                );

                UserStates.TryRemove(userId, out _);
            }

            //Verify user status to remove item from list
            if (UserStates.TryGetValue(userId, out var stateToRemove) && stateToRemove == "waiting_item_to_remove")
            {
                var item = message.Text;

                string methodResponse = _botClient.SendItemToRemoveFromList(item!);

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: methodResponse,
                    cancellationToken: cancellationToken
                );
            }

            //Verify user status to create new list with item
            if (UserStates.TryGetValue(userId, out var state) && state == "waiting_items_to_create_new_list")
            {
                var item = message.Text;
                
                string methodResponse = _botClient.GetItemsToCreatelist(item!);
                 
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: methodResponse,
                    cancellationToken: cancellationToken
                );

                UserStates.TryRemove(userId, out _);
            }

            var startService = _botClient.StartService();
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

                var methodResponse = _botClient.ShowList();
                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: methodResponse,
                    cancellationToken: cancellationToken
                );
            }


            //Verify option 'atualizar lista' and asks the user wich kind atualization
            if (callbackQuery.Data == "Atualizar lista")
            {
                await botClient.AnswerCallbackQuery(
                    callbackQueryId: callbackQuery.Id,
                    text: "Selecione o tipo da atualização.",
                    cancellationToken: cancellationToken
                );

                var listUpdatesOptionsKeyboard = _botClient.GetOptionsOfListUpdate();
                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: "Selecione o tipo de atualização que deseja fazer:",
                    replyMarkup: listUpdatesOptionsKeyboard,
                    cancellationToken: cancellationToken
                );
            }

            //Verify option 'adicionar item' and asks the user
            if (callbackQuery.Data == "Adicionar um item")
            {
                UserStates[userId] = "waiting_item_to_add";

                await botClient.AnswerCallbackQuery(
                    callbackQueryId: callbackQuery.Id,
                    text: $"Digite o nome do item que deseja adicionar.",
                    cancellationToken: cancellationToken
                );
                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: "Ótimo! Para fazer a inserção você pode enviar um ou mais itens se desejar.\nLembrando que apenas o nome é obrigatório, mas é ideal que você informe a Marca e o Preço também para uma melhor experiência!",
                    cancellationToken: cancellationToken
                );
                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: "Vamos lá! Para inserir o item, siga a seguinte extrutura:\nProduto - Marca - Preco\nProduto - Marca - Preco\n\nAgora, por favor, informe os itens que deseja adicionar:",
                    cancellationToken: cancellationToken
                );
            }

            //Verify option 'alterar um item' and asks the user
            if (callbackQuery.Data == "Alterar um item")
            {
                UserStates[userId] = "waiting_for_name_attribute_to_updated";

                await botClient.AnswerCallbackQuery(
                    callbackQueryId: callbackQuery.Id,
                    text: "Digite o nome do item que deseja alterar.",
                    cancellationToken: cancellationToken
                );

                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: "Por favor, informe agora o nome do item que deseja fazer alteração:",
                    cancellationToken: cancellationToken
                );
            }

            if (callbackQuery.Data == "Nome" || callbackQuery.Data == "Marca" || callbackQuery.Data == "Preço")
            {
                UserStates[userId] = "waiting_for_attribute_to_update";

                string verifyGener = callbackQuery.Data[callbackQuery.Data.Length -1] == 'a' ? "a" : "o";
                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: $"Informe {verifyGener} {callbackQuery.Data}:",
                    cancellationToken: cancellationToken
                );
            }

            //Verify option 'remover um item' and asks the user
            if (callbackQuery.Data == "Remover um item")
            {
                UserStates[userId] = "waiting_item_to_remove";

                await botClient.AnswerCallbackQuery(
                    callbackQueryId: callbackQuery.Id,
                    text: "Digite o nome do item que deseja remover.",
                    cancellationToken: cancellationToken
                );

                await botClient.SendMessage(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: "Por favor, informe agora o nome do item que deseja remover:",
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