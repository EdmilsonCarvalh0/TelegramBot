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
        private readonly static UpdateHandlers _handlers = new();
        public TelegramBotClient Bot;
        // private static readonly UserStateManager _userStateManager = new();

        public BotConnection()
        {
            Bot = new TelegramBotClient(Token);
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            long userId = update.CallbackQuery?.From.Id ?? update.Message?.Chat.Id ?? 0;
            
            // _handlers.UserStateManager.SetUserId(userId);

            var context = new BotRequestContext(
                botClient,
                userId,
                update.CallbackQuery,
                update.Message,
                cancellationToken
            );

            _handlers.LoadContext(context);
            await DelegateUpdates(update);
        }

        private static async Task DelegateUpdates(Update update)
        {
            var userState = _handlers.UserStateManager.GetUserStateData(_handlers.Context.UserId);

            if (update.Type == UpdateType.Message && update.Message != null)
            {
                if (userState.State == UserState.None)
                {
                    await _handlers.HandleInitialMessage();
                    Console.WriteLine($"Estado atual {_handlers.UserStateManager.GetUserStateData(_handlers.Context.UserId).State}");
                    return;
                }

                await _handlers.HandleMessageAsync();
                Console.WriteLine($"Estado atual {_handlers.UserStateManager.GetUserStateData(_handlers.Context.UserId).State}");
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                await _handlers.HandleCallbackQueryAsync();
                Console.WriteLine($"Estado atual {_handlers.UserStateManager.GetUserStateData(_handlers.Context.UserId).State}");
            }
        }

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Erro recebido: {exception.Message}\nData: {exception.InnerException}");
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
