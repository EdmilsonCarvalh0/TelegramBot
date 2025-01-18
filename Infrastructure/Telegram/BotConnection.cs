using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application;
using TelegramBot.Data;
using TelegramBot.Domain;
using TelegramBot.Infrastructure.Handlers;

namespace TelegramBot.Infrastructure
{
    public class BotConnection
    {
        public static readonly UserStateManager _userStateManager = new();

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

            //TODO: Implement logic for each user state 
            //      Implementar lógica para cada estado de usuário

            var handlers = new UpdateHandlers(context);
            var userState = _userStateManager.GetState(context.UserId);

            UserState responseState;


            if (update.Type == UpdateType.Message && update.Message != null)
            {
                switch (userState)
                {
                    case UserState.None:
                        responseState = await handlers.HandleInitialMessage(userState);
                        _userStateManager.SetState(context.UserId, responseState);
                        Console.WriteLine($"Estado atual {_userStateManager.GetState(context.UserId)}");
                        return;
                    
                    case UserState.ServicePaused:
                        break;
                    
                }

                responseState = await handlers.HandleMessageAsync();
                _userStateManager.SetState(context.UserId, responseState);
                Console.WriteLine($"Estado atual {_userStateManager.GetState(context.UserId)}");
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                responseState = await handlers.HandleCallbackQueryAsync();
                _userStateManager.SetState(context.UserId, responseState);
                Console.WriteLine($"Estado atual {_userStateManager.GetState(context.UserId)}");
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
}
