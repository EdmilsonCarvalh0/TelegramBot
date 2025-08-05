using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers.Interface;

namespace TelegramBot.Application.Handlers.ShoppingMode
{
    public class ShoppingModeHandlerResolver : IUpdateHandlerResolver
    {
        public bool CanHandle(InteractionState interactionState) => interactionState == InteractionState.ShoppingModeEnabled;

        // TODO: Implementar ShoppingModeHandler
        public IHandler ResolveHandler(HandlerContext context) => new MessageHandler(context);
    }
}
