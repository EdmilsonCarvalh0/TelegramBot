

using Telegram.Bot;
using Telegram.Bot.Types;

public class BotRequestContext
{
    public ITelegramBotClient BotClient { get; }
    public long UserId { get; }
    public CallbackQuery? CallbackQuery { get; }
    public Message? Message { get; }
    public CancellationToken CancellationToken { get; }

    public BotRequestContext(ITelegramBotClient botCLient, long userId,
                            CallbackQuery? callbackQuery, Message? message,
                            CancellationToken cancellationToken)
    {
        BotClient = botCLient;
        UserId = userId;
        CallbackQuery = callbackQuery;
        Message = message;
        CancellationToken = cancellationToken;
    }

    /*
        Avaliar arquitetura para saber onde inserir a classe
        (talvez na de aplicação)
    */
}