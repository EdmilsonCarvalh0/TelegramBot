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
        private readonly ConcurrentDictionary<long, UserStateData> UserStates = new();
        public UserStateManager UserStateManager { get; set; }
        public BotRequestContext Context { get; set; }
        private static readonly IItemController messageHandler = new ItemController();

        public UpdateHandlers()
        {
            UserStates.TryAdd(
                Context!.UserId,
                new UserStateData {
                    State = UserState.None,
                    LastUpdated = DateTime.UtcNow,
                    AdditionalInfo = ""
                }
            );

            UserStateManager = new UserStateManager();
        }

        public void LoadContext(BotRequestContext context)
        {
            Context = context;
        }

        public async Task<UserState> HandleMessageAsync()
        {

            /*
                TODO: refatorar o restante das manipulações de UserStateManager e
                implementar os métodos corretos de manipulação de AdditionalInfo
            */
            var userStateData = UserStateManager.GetUserStateData(Context.UserId);

            switch (userStateData.AdditionalInfo)
            {
                case "waiting_item_to_add":
                    return await HandleWaitingToAddOfItem();
                
                case "waiting_for_attribute_to_update":
                    return await HandleWaitingForAttributeToUpdated();

                case "waiting_for_name_attribute_to_update":
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

        private async Task<UserState> SendResponseToUser(ResponseContent responseContent)
        {
            if (responseContent.KeyboardMarkup != null)
            {
                await Context.BotClient.SendMessage(
                    chatId: Context.UserId,
                    text: responseContent.Text,
                    replyMarkup: responseContent.KeyboardMarkup,
                    cancellationToken: Context.CancellationToken
                );

                return responseContent.UserState;
            }

            await Context.BotClient.SendMessage(
                chatId: Context.UserId,
                text: responseContent.Text,
                cancellationToken: Context.CancellationToken
            );

            return responseContent.UserState;
        }

        private async Task<UserState> SendMenuOfBot()
        {
            var responseContent = messageHandler.StartService();
            return await SendResponseToUser(responseContent);
        }

        public async Task<UserState> HandleInitialMessage(UserState state)
        {
            if (state == UserState.None)
            {
                if (Context.Message?.Text != "Menu")
                {
                    var responseContent = messageHandler.GetInitialMessage();
                    return await SendResponseToUser(responseContent);
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
            UserStates.TryRemove(Context.UserId, out _);
            var item = Context.Message!.Text;
            var responseContent = messageHandler.AddItemInShoppingData(item!);
            return await SendResponseToUser(responseContent);
        }

        private async Task<UserState> HandleWaitingForAttributeToUpdated()
        {
            UserStates.TryRemove(Context.UserId, out _);
            var attribute = Context.Message!.Text;
            var responseContent = messageHandler.SendItemToUpdateList(attribute!);
            return await SendResponseToUser(responseContent);
        }

        private async Task<UserState> HandleWaitingForNameAttributeToUpdated()
        {
            var nameAttribute = Context.Message!.Text;
            var responseContent = messageHandler.CheckItemExistence(nameAttribute!);

            if (responseContent.UserState == UserState.UpdateList)
            {
                UserStates[Context.UserId].AdditionalInfo = "waiting_for_name_attribute_to_update";
                return await SendResponseToUser(responseContent);
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

            string genderVerified = nameAttribute![nameAttribute.Length - 1] == 'a' ? "da" : "do";
            response.Text += genderVerified + nameAttribute + "?";
            UserStates.TryRemove(Context.UserId, out _);
            return await SendResponseToUser(response);
        }

        private async Task<UserState> HandleWaitingItemToRemove()
        {
            var item = Context.Message!.Text;
            var responseContent = messageHandler.SendItemToRemoveFromList(item!);
            return await SendResponseToUser(responseContent);
        }

        private async Task<UserState> HandleWaitingItemsCreatingNewList()
        {
            UserStates.TryRemove(Context.UserId, out _);
            var item = Context.Message!.Text;
            var responseContent = messageHandler.GetItemsToCreatelist(item!);                 
            return await SendResponseToUser(responseContent);
        }

        public async Task<UserState> HandleSendOfList()
        {
            await Context.BotClient.AnswerCallbackQuery(
                callbackQueryId: Context.CallbackQuery!.Id,
                text: "Enviando lista...",
                cancellationToken: Context.CancellationToken
            );

            var responseContent = messageHandler.ShowList();
            return await SendResponseToUser(responseContent);
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

            var responseContemt = messageHandler.GetOptionsOfListUpdate();
            await Context.BotClient.SendMessage(
                chatId: Context.UserId,
                text: "Selecione o tipo de atualização que deseja fazer:",
                replyMarkup: responseContemt.KeyboardMarkup,
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
            UserStates[Context.UserId].AdditionalInfo = "waiting_for_name_attribute_to_update";

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
}
