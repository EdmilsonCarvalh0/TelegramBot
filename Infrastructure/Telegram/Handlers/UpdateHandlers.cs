using System.Collections.Concurrent;
using Microsoft.AspNetCore.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Application;
using TelegramBot.Core;
using TelegramBot.Domain;

namespace TelegramBot.Infrastructure.Handlers;

public class UpdateHandlers
{
    private static readonly ConcurrentDictionary<long, UserStateData> UserStates = new();
    private readonly BotRequestContext Context;
    private static readonly IItemController messageHandler = new ItemController();

    public UpdateHandlers(BotRequestContext context)
    {
        Context = context;
        UserStates.TryAdd(
            context.UserId,
            new UserStateData {
                State = UserState.None,
                LastUpdated = DateTime.UtcNow,
                AdditionalInfo = ""
            }
        );
    }

    public async Task<UserState> HandleMessageAsync()
    {
        switch (UserStates[Context.UserId].AdditionalInfo)
        {
            case "waiting_item_to_add":
                return await HandleWaitingToAddOfItem();
                
            case "waiting_for_attribute_to_updated":
                return await HandleWaitingForAttributeToUpdated();

            case "waiting_for_name_attribute_to_updated":
                return await HandleWaitingForNameAttributeToUpdated();
                
            case "waiting_item_to_remove":
                return await HandleWaitingItemToRemove();

            case "waiting_items_to_create_new_list":
                return await HandleWaitingItemsCreatingNewList();
        }

        Console.WriteLine($"{Context.Message!.From?.Username} saying: {Context.Message.Text}");
        return UserState.None;
    }

    public async Task<UserState> HandleCallbackQueryAsync()
    {
        if (Context.CallbackQuery == null) return UserState.None;

        switch (Context.CallbackQuery.Data)
        {
            //Verify option 'ver lista' and send list to user
            case "Ver lista":
                return await HandleSendOfList();
                
            //Verify option 'atualizar lista' and asks the user wich kind atualization
            case "Atualizar lista":
                return await HandleToUpdateList();

                //Verify option 'adicionar item' and asks the user
            case "Adicionar um item":
                return await HandleToAddItem();

            //Verify option 'alterar um item' and asks the user
            case "Alterar um item":
                return await HandleItemChange();

            case "Nome":
                return await HandleAttributeChange();
                
            case "Marca":
                return await HandleAttributeChange();
                
            case "Preço":
                return await HandleAttributeChange();

                //Verify option 'remover um item' and asks the user
            case "Remover um item":
                return await HandleItemRemove();
            
            case "Criar nova lista":
            //Verify option 'criar nova lista' 
                return await HandleCreatingNewList();
        }

        return UserState.None;
    }

    private async Task<UserState> SendMenuOfBot()
    {
        var menuButtons = messageHandler.StartService();
        await Context.BotClient.SendMessage(
            chatId: Context.Message!.Chat.Id,
            text: $"Olá, bem vindo ao Bot de Compras Mensais!\nEscolha uma opção:",
            replyMarkup: menuButtons,
            cancellationToken: Context.CancellationToken
        );

        return UserState.Running;

    }

    public async Task<UserState> HandleInitialMessage(UserState state)
    {
        if (state == UserState.None)
        {
            if (Context.Message?.Text != "Menu")
            {
                await Context.BotClient.SendMessage(
                    Context.UserId,
                    "Para ir para o menu inicial digite 'Menu'.",
                    cancellationToken: Context.CancellationToken
                );
                return UserState.None;
            }

            if (Context.Message?.Text == "Menu")
            {
                return await SendMenuOfBot();
            }
        }

        return UserState.None;
    }

    private async Task<UserState> HandleWaitingToAddOfItem()
    {
        var item = Context.Message!.Text;

        string methodResponse = messageHandler.AddItemInShoppingData(item!);
                
        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: methodResponse,
            cancellationToken: Context.CancellationToken
        );

        UserStates.TryRemove(Context.UserId, out _);
        return UserState.None;
    }

    private async Task<UserState> HandleWaitingForAttributeToUpdated()
    {
        var attribute = Context.Message!.Text;

        string methodResponse = messageHandler.SendItemToUpdateList(attribute!);
        await Context.BotClient.SendMessage(
            chatId: Context.Message.Chat.Id,
            text: methodResponse,
            cancellationToken: Context.CancellationToken
        );

        UserStates.TryRemove(Context.UserId, out _);
        return UserState.None;
    }

    private async Task<UserState> HandleWaitingForNameAttributeToUpdated()
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
        return UserState.UpdateItem;
    }

    private async Task<UserState> HandleWaitingItemToRemove()
    {
        var item = Context.Message!.Text;

        string methodResponse = messageHandler.SendItemToRemoveFromList(item!);

        await Context.BotClient.SendMessage(
            chatId: Context.Message.Chat.Id,
            text: methodResponse,
            cancellationToken: Context.CancellationToken
        );

        return UserState.None;
    }

    private async Task<UserState> HandleWaitingItemsCreatingNewList()
    {
        var item = Context.Message!.Text;
                
        string methodResponse = messageHandler.GetItemsToCreatelist(item!);
                 
        await Context.BotClient.SendMessage(
            chatId: Context.Message.Chat.Id,
            text: methodResponse,
            cancellationToken: Context.CancellationToken
        );

        UserStates.TryRemove(Context.UserId, out _);
        return UserState.None;
    }

    public async Task<UserState> HandleSendOfList()
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

        return UserState.None;
    }

    public async Task<UserState> HandleToUpdateList()
    {
        UserStates[Context.UserId] = new UserStateData {
            State = UserState.UpdateList,
            LastUpdated = DateTime.UtcNow,
            AdditionalInfo = ""
        };

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

        return UserState.UpdateList;
    }

    public async Task<UserState> HandleToAddItem()
    {
        UserStates[Context.UserId].AdditionalInfo = "waiting_item_to_add";

        await Context.BotClient.AnswerCallbackQuery(
            callbackQueryId: Context.CallbackQuery!.Id,
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

        return UserState.AddItem;
    }

    public async Task<UserState> HandleItemChange()
    {
        UserStates[Context.UserId].AdditionalInfo = "waiting_for_name_attribute_to_updated";

        await Context.BotClient.AnswerCallbackQuery(
            callbackQueryId: Context.CallbackQuery!.Id,
            text: "Digite o nome do item que deseja alterar.",
            cancellationToken: Context.CancellationToken
        );

        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: "Por favor, informe agora o nome do item que deseja fazer alteração:",
            cancellationToken: Context.CancellationToken
        );
        
        return UserState.UpdateItem;
    }

    public async Task<UserState> HandleCreatingNewList()
    {
        UserStates[Context.UserId].AdditionalInfo = "waiting_items_to_create_new_list";

        await Context.BotClient.AnswerCallbackQuery(
            callbackQueryId: Context.CallbackQuery!.Id,
            text: "Informe os itens da nova lista.",
            cancellationToken: Context.CancellationToken
        );
        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: "Envie agora os itens da nova lista.\nUse o padrão adequado para registro correto dos itens. Nesse caso, separe os itens por vírgula sem espaços.\nEx: Pão,Manteiga,Queijo",
            cancellationToken: Context.CancellationToken
        );

        return UserState.CreatingNewList;
    }

    public async Task<UserState> HandleAttributeChange()
    {
        UserStates[Context.UserId].AdditionalInfo = "waiting_for_attribute_to_update";

        string genderVerified = Context.CallbackQuery!.Data![Context.CallbackQuery.Data.Length -1] == 'a' ? "a" : "o";
        await Context.BotClient.SendMessage(
            chatId: Context.UserId,
            text: $"Informe {genderVerified} {Context.CallbackQuery.Data}:",
            cancellationToken: Context.CancellationToken
        );

        return UserState.UpdateItem;
    }

    public async Task<UserState> HandleItemRemove()
    {
        UserStates[Context.UserId].AdditionalInfo = "waiting_item_to_remove";

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

        return UserState.DeleteItem;
    }
}