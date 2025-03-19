
namespace TelegramBot.Infrastructure;

public class ChatIdIdentifier
{
    public long BotId { get; }

    public ChatIdIdentifier(long botId)
    {
        BotId = botId;
    }
    
}