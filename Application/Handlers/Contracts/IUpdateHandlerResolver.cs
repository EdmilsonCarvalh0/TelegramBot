using Telegram.Bot.Types;
using TelegramBot.Application.DTOs;

namespace TelegramBot.Application.Handlers.Interface
{
    public interface IUpdateHandlerResolver
    {
        bool CanHandle(InteractionState interactionState);
        IHandler ResolveHandler(HandlerContext context);
    }
}
