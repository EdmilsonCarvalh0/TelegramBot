using System.Collections.Concurrent;
using Microsoft.AspNetCore.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Application;
using TelegramBot.UserInterface;
using TelegramBot.Domain;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Infrastructure.Handlers
{
    public class UpdateHandlers
    {
        /*
            Avaliar uso dos dados de update posteriormente
            (com qual intuito reter esses dados?)
        */
        // private readonly ConcurrentDictionary<long, UserStateData> UserStates = new();
        public UserStateManager UserStateManager { get; set; }
        public BotRequestContext Context { get; set; } = default!;
        private static readonly IItemController messageHandler = new ItemController();

        public UpdateHandlers()
        {
            // Avaliar o uso do UserStates
            // UserStates.TryAdd(
            //     Context!.UserId,
            //     new UserStateData {
            //         State = UserState.None,
            //         LastUpdated = DateTime.UtcNow,
            //         AdditionalInfo = ""
            //     }
            // );

            UserStateManager = new UserStateManager();
        }

        public void LoadContext(BotRequestContext context)
        {
            Context = context;

            if (!UserStateManager.ContainsUserId(Context.UserId))
            {
                UserStateManager.SetUserId(Context.UserId);
            }
        }

        public async Task HandleMessageAsync()
        {

            /*
                TODO: refatorar o restante das manipulações de UserStateManager e
                implementar os métodos corretos de manipulação de AdditionalInfo
            */
            var userStateData = UserStateManager.GetUserStateData(Context.UserId);

            switch (userStateData.AdditionalInfo)
            {
                case "waiting_item_to_add":
                    await HandleWaitingToAddOfItem();
                    break;
                
                case "waiting_for_attribute_to_update":
                    await HandleWaitingForAttributeToUpdated();
                    break;

                case "waiting_for_name_attribute_to_update":
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

        private async Task SendResponseToUser(ResponseContent responseContent)
        {
            
            if (responseContent.KeyboardMarkup != null)
            {
                await Context.BotClient.SendMessage(
                    chatId: Context.UserId,
                    text: responseContent.Text,
                    replyMarkup: responseContent.KeyboardMarkup,
                    cancellationToken: Context.CancellationToken
                );

                UserStateManager.SetState(Context.UserId, responseContent.UserState);
                return;
            }

            await Context.BotClient.SendMessage(
                chatId: Context.UserId,
                text: responseContent.Text,
                cancellationToken: Context.CancellationToken
            );

            UserStateManager.SetState(Context.UserId, responseContent.UserState);
            return;
        }

        public async Task HandleInitialMessage()
        {
            if (Context.Message?.Text != "Menu")
            {
                var responseContent = messageHandler.GetInitialMessage();
                await SendResponseToUser(responseContent);
            }

            if (Context.Message?.Text == "Menu")
            {
                var responseContent = messageHandler.StartService();
                await SendResponseToUser(responseContent);
            }
        }

        private async Task HandleWaitingToAddOfItem()
        {
            // UserStates.TryRemove(Context.UserId, out _);
            UserStateManager.ResetAdditionalInfo(Context.UserId);
            var item = Context.Message!.Text;
            var responseContent = messageHandler.AddItemInShoppingData(item!);
            await SendResponseToUser(responseContent);
        }

        private async Task HandleWaitingForAttributeToUpdated()
        {
            // UserStates.TryRemove(Context.UserId, out _);
            UserStateManager.ResetAdditionalInfo(Context.UserId);
            var attribute = Context.Message!.Text;
            var responseContent = messageHandler.SendItemToUpdateList(attribute!);
            await SendResponseToUser(responseContent);
        }

        private async Task HandleWaitingForNameAttributeToUpdated()
        {
            var nameAttribute = Context.Message!.Text;
            var responseContent = messageHandler.CheckItemExistence(nameAttribute!);

            if (responseContent.UserState == UserState.UpdateList)
            {
                // UserStates[Context.UserId].AdditionalInfo = "waiting_for_name_attribute_to_update";
                UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_for_name_attribute_to_update");
                await SendResponseToUser(responseContent);
                return;
            }

            //if (responseContent.AdditionalResponseContext == "Item no exist")
            //{
            //    UserStates[Context.UserId].AdditionalInfo = "waiting_for_name_attribute_to_update";
            //    return await SendResponseToUser(responseContent);
            //}
            
            //if(responseContent.Text != string.Empty)
            //{
            //    if (responseContent.Text.Contains('\n'))
            //    {
            //        UserStates[Context.UserId].AdditionalInfo = "waiting_for_name_attribute_to_update";
            //        return await SendResponseToUser(responseContent);
            //    };

            //    await Context.BotClient.SendMessage(Context.UserId, responseContent.Text, cancellationToken: Context.CancellationToken);
            //}

            var response = messageHandler.GetAttributeOptions();

            string genderVerified = nameAttribute![nameAttribute.Length - 1] == 'a' ? "da " : "do ";
            response.Text += genderVerified + nameAttribute + "?";
            // UserStates.TryRemove(Context.UserId, out _);
            UserStateManager.ResetAdditionalInfo(Context.UserId);
            await SendResponseToUser(response);
        }

        private async Task HandleWaitingItemToRemove()
        {
            var item = Context.Message!.Text;
            var responseContent = messageHandler.SendItemToRemoveFromList(item!);
            await SendResponseToUser(responseContent);
        }

        private async Task HandleWaitingItemsCreatingNewList()
        {
            // UserStates.TryRemove(Context.UserId, out _);
            UserStateManager.ResetAdditionalInfo(Context.UserId);
            var item = Context.Message!.Text;
            var responseContent = messageHandler.GetItemsToCreatelist(item!);                 
            await SendResponseToUser(responseContent);
        }

        public async Task HandleSendOfList()
        {
            await Context.BotClient.AnswerCallbackQuery(
                callbackQueryId: Context.CallbackQuery!.Id,
                text: "Enviando lista...",
                cancellationToken: Context.CancellationToken
            );

            var responseContent = messageHandler.ShowList();
            await SendResponseToUser(responseContent);
        }

        public async Task HandleToUpdateList()
        {
            // UserStates[Context.UserId] = new UserStateData {
            //     State = UserState.UpdateList,
            //     LastUpdated = DateTime.UtcNow,
            //     AdditionalInfo = ""
            // };

            await Context.BotClient.AnswerCallbackQuery(
                callbackQueryId: Context.CallbackQuery!.Id,
                text: "Selecione o tipo da atualização.",
                cancellationToken: Context.CancellationToken
            );

            var responseContent = messageHandler.GetOptionsOfListUpdate();
            await Context.BotClient.SendMessage(
                chatId: Context.UserId,
                text: "Selecione o tipo de atualização que deseja fazer:",
                replyMarkup: responseContent.KeyboardMarkup,
                cancellationToken: Context.CancellationToken
            );

            UserStateManager.SetState(Context.UserId, UserState.UpdateList);
        }

        public async Task HandleToAddItem()
        {
            // UserStates[Context.UserId].AdditionalInfo = "waiting_item_to_add";

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

            UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_item_to_add");
            UserStateManager.SetState(Context.UserId, UserState.AddItem);
        }

        public async Task HandleItemChange()
        {
            // UserStates[Context.UserId].AdditionalInfo = "waiting_for_name_attribute_to_update";

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
        
            UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_for_name_attribute_to_update");
            UserStateManager.SetState(Context.UserId, UserState.UpdateItem);
        }

        public async Task HandleCreatingNewList()
        {
            // UserStates[Context.UserId].AdditionalInfo = "waiting_items_to_create_new_list";

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

            UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_items_to_create_new_list");
            UserStateManager.SetState(Context.UserId, UserState.CreatingNewList);
        }

        public async Task HandleAttributeChange()
        {
            // UserStates[Context.UserId].AdditionalInfo = "waiting_for_attribute_to_update";

            string genderVerified = Context.CallbackQuery!.Data![Context.CallbackQuery.Data.Length -1] == 'a' ? "a" : "o";
            await Context.BotClient.SendMessage(
                chatId: Context.UserId,
                text: $"Informe {genderVerified} {Context.CallbackQuery.Data}:",
                cancellationToken: Context.CancellationToken
            );

            UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_for_attribute_to_update");
            UserStateManager.SetState(Context.UserId, UserState.UpdateItem);
        }

        public async Task HandleItemRemove()
        {
            // UserStates[Context.UserId].AdditionalInfo = "waiting_item_to_remove";

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

            UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_item_to_remove");
            UserStateManager.SetState(Context.UserId, UserState.DeleteItem);
        }
    }
}
