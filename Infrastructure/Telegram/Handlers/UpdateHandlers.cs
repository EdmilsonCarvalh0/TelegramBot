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
        public UserStateManager UserStateManager { get; set; }
        public BotRequestContext Context { get; set; } = default!;
        private static readonly IItemController messageHandler = new ItemController();

        public UpdateHandlers()
        {
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
            switch (Context.CallbackQuery!.Data)
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

        private async Task SendResponseToUser(ResponseContentDTO responseContent)
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
                var responseContent = messageHandler.GetInitialMessage("Initial Message");
                await SendResponseToUser(responseContent);
            }

            if (Context.Message?.Text == "Menu")
            {
                var responseContent = messageHandler.StartService(Context.Message.Text);
                await SendResponseToUser(responseContent);
            }
        }

        private async Task HandleWaitingToAddOfItem()
        {
            UserStateManager.ResetAdditionalInfo(Context.UserId);
            var item = Context.Message!.Text;
            var responseContent = messageHandler.AddItemInShoppingData(item!);
            await SendResponseToUser(responseContent);
        }

        private async Task HandleWaitingForAttributeToUpdated()
        {
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
                UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_for_name_attribute_to_update");
                await SendResponseToUser(responseContent);
                return;
            }

            string genderVerified = CheckAttributeGender(nameAttribute!);
            responseContent.Text += genderVerified + nameAttribute + "?";
            UserStateManager.ResetAdditionalInfo(Context.UserId);
            await SendResponseToUser(responseContent);
        }

        private async Task HandleWaitingItemToRemove()
        {
            var item = Context.Message!.Text;
            var responseContent = messageHandler.SendItemToRemoveFromList(item!);
            await SendResponseToUser(responseContent);
        }

        private async Task HandleWaitingItemsCreatingNewList()
        {
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
            await Context.BotClient.AnswerCallbackQuery(
                callbackQueryId: Context.CallbackQuery!.Id,
                text: "Selecione o tipo da atualização.",
                cancellationToken: Context.CancellationToken
            );

            var responseContent = messageHandler.GetOptionsOfListUpdate(Context.CallbackQuery.Data!);
            await SendResponseToUser(responseContent);

            UserStateManager.SetState(Context.UserId, UserState.UpdateList);
        }

        public async Task HandleToAddItem()
        {
            await Context.BotClient.AnswerCallbackQuery(
                callbackQueryId: Context.CallbackQuery!.Id,
                text: $"Digite o nome do item que deseja adicionar.",
                cancellationToken: Context.CancellationToken
            );

            var responseContent = messageHandler.GetResponseCallback(Context.CallbackQuery.Data!);
            await SendResponseToUser(responseContent);

            UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_item_to_add");
            UserStateManager.SetState(Context.UserId, UserState.AddItem);
        }

        public async Task HandleItemChange()
        {
            await Context.BotClient.AnswerCallbackQuery(
                callbackQueryId: Context.CallbackQuery!.Id,
                text: "Digite o nome do item que deseja alterar.",
                cancellationToken: Context.CancellationToken
            );

            var responseContent = messageHandler.GetResponseCallback(Context.CallbackQuery.Data!);
            await SendResponseToUser(responseContent);
        
            UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_for_name_attribute_to_update");
            UserStateManager.SetState(Context.UserId, UserState.UpdateItem);
        }

        public async Task HandleCreatingNewList()
        {
            await Context.BotClient.AnswerCallbackQuery(
                callbackQueryId: Context.CallbackQuery!.Id,
                text: "Informe os itens da nova lista.",
                cancellationToken: Context.CancellationToken
            );
            
            var responseContent = messageHandler.GetResponseCallback(Context.CallbackQuery.Data!);
            await SendResponseToUser(responseContent);

            UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_items_to_create_new_list");
            UserStateManager.SetState(Context.UserId, UserState.CreateList);
        }

        public async Task HandleAttributeChange()
        {
            string genderVerified = CheckItemGender(Context.CallbackQuery!.Data!);
            var responseContent = messageHandler.GetResponseMessage("Atributte Change");
            responseContent.Text += $"{genderVerified} {Context.CallbackQuery!.Data}:";

            await SendResponseToUser(responseContent);

            UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_for_attribute_to_update");
            UserStateManager.SetState(Context.UserId, UserState.UpdateItem);
        }

        public async Task HandleItemRemove()
        {
            await Context.BotClient.AnswerCallbackQuery(
                callbackQueryId: Context.CallbackQuery!.Id,
                text: "Digite o nome do item que deseja remover.",
                cancellationToken: Context.CancellationToken
            );

            var responseContent = messageHandler.GetResponseCallback(Context.CallbackQuery.Data!);
            await SendResponseToUser(responseContent);

            UserStateManager.SetAdditionalInfo(Context.UserId, "waiting_item_to_remove");
            UserStateManager.SetState(Context.UserId, UserState.DeleteItem);
        }

        private string CheckItemGender(string item)
        {
            return item[item.Length -1] == 'a' ? "a" : "o";
        }

        private string CheckAttributeGender(string item)
        {
            return item[item.Length - 1] == 'a' ? "da " : "do ";
        }
    }
}
