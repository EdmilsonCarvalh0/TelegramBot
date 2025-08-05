using TelegramBot.Application.Bot;

namespace TelegramBot.Infrastructure.Telegram
{
    public interface IMessageSender
    {
        Task SendMessageAsync(ResponseContent responseContent, long userId, CancellationToken cancellationToken);
    }
}
