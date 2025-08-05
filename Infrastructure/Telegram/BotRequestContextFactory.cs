
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

    // TODO: Verificar os casos de uso onde o usuário pode enviar um update diferente de message e callback
    // Open Closed Principle?
    public BotRequestContext Create(long userId, Update update)
    {
        ArgumentNullException.ThrowIfNull(update, nameof(update));

        return new BotRequestContext(new ChatIdIdentifier(userId), 
                                     update.CallbackQuery,
                                     update.Message);
    }
}