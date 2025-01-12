using System.Collections.Concurrent;
using Microsoft.AspNetCore.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Core;

namespace TelegramBot.Infrastructure.Handlers;

public class UpdateHandlers
{
    private static readonly ConcurrentDictionary<long, string> UserStates = new();
    private readonly BotRequestContext Context;
    private static readonly IMessageHandler messageHandler = new MessageHandler();

    public UpdateHandlers(BotRequestContext context)
    {
        Context = context;
        UserStates.TryAdd(context.UserId, "");
    }

    public async Task HandleMessageAsync()
    {
        InitialMessageResult result = await HandleInitialMessage();
        
        if (result == InitialMessageResult.InvalidInput && UserStates[Context.UserId] == "service_paused") return;

        switch (UserStates[Context.UserId])
        {
            case "waiting_item_to_add":
            await HandleWaitingToAddOfItem();
            break;
                
            case "waiting_for_attribute_to_updated":
                await HandleWaitingForAttributeToUpdated();
                break;

            case "waiting_for_name_attribute_to_updated":
                await HandleWaitingForNameAttributeToUpdated();
                break;
                
            case "waiting_item_to_remove":
                await HandleWaitingItemToRemove();
                break;

            case "waiting_items_to_create_new_list":
                await HandleWaitingItemsCreatingNewList();
                break;
        }

        Console.WriteLine($"{Context.Message!.From?.Username} saying: {Context.Message.Text}");

    }

    public async Task HandleCallbackQueryAsync()
    {
        if (Context.CallbackQuery == null) return;

        switch (Context.CallbackQuery.Data)
        {
            //Verify option 'ver lista' and send list to user
            case "Ver lista":
                await HandleSendOfList();
                break;
                
            //Verify option 'atualizar lista' and asks the user wich kind atualization
            case "Atualizar lista":
                await HandleToUpdateList();
                break;

                //Verify option 'adicionar item' and asks the user
            case "Adicionar um item":
                await HandleToAddItem();
                break;

            //Verify option 'alterar um item' and asks the user
            case "Alterar um item":
                await HandleItemChange();
                break;

            case "Nome":
                await HandleAttributeChange();
                break;
                
            case "Marca":
                await HandleAttributeChange();
                break;
                
            case "Preço":
                await HandleAttributeChange();
                break;

                //Verify option 'remover um item' and asks the user
            case "Remover um item":
                await HandleItemRemove();
                break;
            
            case "Criar nova lista":
            //Verify option 'criar nova lista' 
                await HandleCreatingNewList();
                break;
        }
    }

    private async Task SendMenuOfBot()
    {
        var initialButtons = messageHandler.StartService();
        await Context.BotClient.SendMessage(
            chatId: Context.Message!.Chat.Id,
            text: $"Olá, bem vindo ao Bot de Compras Mensais!\nEscolha uma opção:",
            replyMarkup: initialButtons,
            cancellationToken: Context.CancellationToken
        );
    }

    private async Task<InitialMessageResult> HandleInitialMessage()
    {
        if (UserStates[Context.UserId] == "")
        {
            UserStates!.TryAdd(Context.UserId, "service_paused");

            if (Context.Message?.Text != "Menu")
            {
                await Context.BotClient.SendMessage(
                    Context.UserId,
                    "Para ir para o menu inicial digite 'Menu'.",
                    cancellationToken: Context.CancellationToken
                );
                return InitialMessageResult.InvalidInput;
            }

            if (Context.Message?.Text == "Menu")
            {
                UserStates.TryAdd(Context.UserId, "service_started");
                await SendMenuOfBot();
                return InitialMessageResult.MenuPlayed;
            }
        }

        return InitialMessageResult.Unknown;
    }

    private  async Task HandleWaitingToAddOfItem()
    {
        var item = Context.Message!.Text;

        string methodResponse = messageHandler.AddItemInShoppingData(item!);
                
        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: methodResponse,
            cancellationToken: Context.CancellationToken
        );

        UserStates.TryRemove(Context.UserId, out _);
    }

    private  async Task HandleWaitingForAttributeToUpdated()
    {
        var attribute = Context.Message!.Text;

        string methodResponse = messageHandler.SendItemToUpdateList(attribute!);
        await Context.BotClient.SendMessage(
            chatId: Context.Message.Chat.Id,
            text: methodResponse,
            cancellationToken: Context.CancellationToken
        );

        UserStates.TryRemove(Context.UserId, out _);
    }

    private async Task HandleWaitingForNameAttributeToUpdated()
    {
        var attributeName = Context.Message!.Text;

        var attributeOptionsKeyboard = messageHandler.GetAttributeOptions();
        await Context.BotClient.SendMessage(
            chatId: Context.Message.Chat.Id,
            text: "O que deseja alterar?",
            replyMarkup: attributeOptionsKeyboard,
            cancellationToken: Context.CancellationToken
        );

        UserStates.TryRemove(Context.UserId, out _);
    }

    private async Task HandleWaitingItemToRemove()
    {
        var item = Context.Message!.Text;

        string methodResponse = messageHandler.SendItemToRemoveFromList(item!);

        await Context.BotClient.SendMessage(
            chatId: Context.Message.Chat.Id,
            text: methodResponse,
            cancellationToken: Context.CancellationToken
        );
    }

    private async Task HandleWaitingItemsCreatingNewList()
    {
        var item = Context.Message!.Text;
                
        string methodResponse = messageHandler.GetItemsToCreatelist(item!);
                 
        await Context.BotClient.SendMessage(
            chatId: Context.Message.Chat.Id,
            text: methodResponse,
            cancellationToken: Context.CancellationToken
        );

        UserStates.TryRemove(Context.UserId, out _);
    }

    public async Task HandleSendOfList()
    {
        await Context.BotClient.AnswerCallbackQuery(
            callbackQueryId: Context.CallbackQuery!.Id,
            text: "Enviando lista...",
            cancellationToken: Context.CancellationToken
        );

        var methodResponse = messageHandler.ShowList();
        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: methodResponse,
            cancellationToken: Context.CancellationToken
        );
    }

    public async Task HandleToUpdateList()
    {
        await Context.BotClient.AnswerCallbackQuery(
            callbackQueryId: Context.CallbackQuery!.Id,
            text: "Selecione o tipo da atualização.",
            cancellationToken: Context.CancellationToken
        );

        var listUpdatesOptionsKeyboard = messageHandler.GetOptionsOfListUpdate();
        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: "Selecione o tipo de atualização que deseja fazer:",
            replyMarkup: listUpdatesOptionsKeyboard,
            cancellationToken: Context.CancellationToken
        );
    }

    public async Task HandleToAddItem()
    {
        UserStates[Context.CallbackQuery!.From.Id] = "waiting_item_to_add";

        await Context.BotClient.AnswerCallbackQuery(
            callbackQueryId: Context.CallbackQuery.Id,
            text: $"Digite o nome do item que deseja adicionar.",
            cancellationToken: Context.CancellationToken
        );
        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: "Ótimo! Para fazer a inserção você pode enviar um ou mais itens se desejar.\nLembrando que apenas o nome é obrigatório, mas é ideal que você informe a Marca e o Preço também para uma melhor experiência!",
            cancellationToken: Context.CancellationToken
        );
        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: "Vamos lá! Para inserir o item, siga a seguinte extrutura:\nProduto - Marca - Preco\nProduto - Marca - Preco\n\nAgora, por favor, informe os itens que deseja adicionar:",
            cancellationToken: Context.CancellationToken
        );
    }

    public async Task HandleItemChange()
    {
        UserStates[Context.CallbackQuery!.From.Id] = "waiting_for_name_attribute_to_updated";

        await Context.BotClient.AnswerCallbackQuery(
            callbackQueryId: Context.CallbackQuery.Id,
            text: "Digite o nome do item que deseja alterar.",
            cancellationToken: Context.CancellationToken
        );

        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: "Por favor, informe agora o nome do item que deseja fazer alteração:",
            cancellationToken: Context.CancellationToken
        );
    }

    public async Task HandleCreatingNewList()
    {
        UserStates[Context.CallbackQuery!.From.Id] = "waiting_items_to_create_new_list";

        await Context.BotClient.AnswerCallbackQuery(
            callbackQueryId: Context.CallbackQuery.Id,
            text: "Informe os itens da nova lista.",
            cancellationToken: Context.CancellationToken
        );
        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: "Envie agora os itens da nova lista.\nUse o padrão adequado para registro correto dos itens. Nesse caso, separe os itens por vírgula sem espaços.\nEx: Pão,Manteiga,Queijo",
            cancellationToken: Context.CancellationToken
        );
    }

    public async Task HandleAttributeChange()
    {
        UserStates[Context.CallbackQuery!.From.Id] = "waiting_for_attribute_to_update";

        string genderVerified = Context.CallbackQuery.Data![Context.CallbackQuery.Data.Length -1] == 'a' ? "a" : "o";
        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: $"Informe {genderVerified} {Context.CallbackQuery.Data}:",
            cancellationToken: Context.CancellationToken
        );
    }

    public async Task HandleItemRemove()
    {
        UserStates[Context.UserId] = "waiting_item_to_remove";

        await Context.BotClient.AnswerCallbackQuery(
            callbackQueryId: Context.CallbackQuery!.Id,
            text: "Digite o nome do item que deseja remover.",
            cancellationToken: Context.CancellationToken
        );

        await Context.BotClient.SendMessage(
            chatId: Context.CallbackQuery.Message!.Chat.Id,
            text: "Por favor, informe agora o nome do item que deseja remover:",
            cancellationToken: Context.CancellationToken
        );
    }
}