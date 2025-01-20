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

        //TODO: Corrigir erro de referência nula no handlers
        private readonly static string Token = "7560368958:AAGSWm6chmVviBNYSNF8P4Yh3aJdcka0vQw";
        private static UpdateHandlers _handlers;
        public TelegramBotClient Bot;
        private static readonly UserStateManager _userStateManager = new();

        public BotConnection()
        {
            Bot = new TelegramBotClient(Token);
        }

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

            _handlers.LoadContext(context);
            await DelegateUpdates(_handlers, update);
        }

        private static async Task DelegateUpdates(UpdateHandlers handlers,  Update update)
        {
            var userState = _userStateManager.GetState(_handlers.Context.UserId);

            UserState responseState;


            if (update.Type == UpdateType.Message && update.Message != null)
            {
                if (userState == UserState.None)
                {
                    responseState = await handlers.HandleInitialMessage(userState);
                    _userStateManager.SetState(_handlers.Context.UserId, responseState);
                    Console.WriteLine($"Estado atual {_userStateManager.GetState(_handlers.Context.UserId)}");
                    return;
                }

                responseState = await handlers.HandleMessageAsync();
                _userStateManager.SetState(_handlers.Context.UserId, responseState);
                Console.WriteLine($"Estado atual {_userStateManager.GetState(_handlers.Context.UserId)}");
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                responseState = await handlers.HandleCallbackQueryAsync();
                _userStateManager.SetState(_handlers.Context.UserId, responseState);
                Console.WriteLine($"Estado atual {_userStateManager.GetState(_handlers.Context.UserId)}");
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
