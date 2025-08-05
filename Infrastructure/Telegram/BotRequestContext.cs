using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Infrastructure;

public class BotRequestContext
{
    public long UserId { get; }
    public CallbackQuery? CallbackQuery { get; }
    public Message? Message { get; }

    public BotRequestContext(ChatIdIdentifier chatIdIdentifier,
                            CallbackQuery? callbackQuery,
                            Message? message)
    {
        UserId = chatIdIdentifier.BotId;
        CallbackQuery = callbackQuery;
        Message = message;
    }
}