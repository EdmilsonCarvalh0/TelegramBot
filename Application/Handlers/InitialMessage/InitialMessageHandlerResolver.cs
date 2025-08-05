using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers.Interface;

namespace TelegramBot.Application.Handlers.InitialMessage;

public class InitialMessageHandlerResolver : IUpdateHandlerResolver
{
    public bool CanHandle(InteractionState interactionState) => interactionState == InteractionState.InitialMessage;

    public IHandler ResolveHandler(HandlerContext context) => new InitialMessageHandler(context);
}
