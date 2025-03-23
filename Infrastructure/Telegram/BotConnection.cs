using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramBot.Infrastructure
{
    public class BotConnection
    {
        private readonly ITelegramBotClient _bot;
        private readonly UpdateHandlers _handlers;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IServiceScopeFactory _scopeFactory;

        public BotConnection(ITelegramBotClient bot, UpdateHandlers handlers, IHostApplicationLifetime appLifetime, IServiceScopeFactory scopeFactory)
        {
            _bot = bot;
            _handlers = handlers;
            _appLifetime = appLifetime;
            _scopeFactory = scopeFactory;
        }

        public void Start()
        {
            _bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>()
                },
                cancellationToken: _appLifetime.ApplicationStopping
            );
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var contextFactory = scope.ServiceProvider.GetRequiredService<BotRequestContextFactory>();

            long userId = update.CallbackQuery?.From.Id ?? update.Message?.Chat.Id ?? 0;
            var context = contextFactory.Create(_bot, userId, update, cancellationToken);

            _handlers.Start(context);

            await _handlers.DelegateUpdates(update);
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Erro recebido: {exception.Message}\nData: {exception.StackTrace}");
            return Task.CompletedTask;
        }

        public async Task DisplayBotInfoAsync()
        {
            var me = await _bot.GetMe();
            Console.WriteLine($"\n--> Bot iniciado: @{me.Username} <--\n");
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
