
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Infrastructure;

public class BotRequestContextFactory
{
    private readonly  IServiceProvider _serviceProvider;

    public BotRequestContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public BotRequestContext Create(ITelegramBotClient botClient, long userId, Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(botClient, nameof(botClient));
        ArgumentNullException.ThrowIfNull(update, nameof(update));

        return new BotRequestContext(botClient,
                                     new ChatIdIdentifier(userId), 
                                     update.CallbackQuery,
                                     update.Message,
                                     cancellationToken);
    }
}