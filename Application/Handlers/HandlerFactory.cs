using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers.Interface;

namespace TelegramBot.Application.Handlers;

public class HandlerFactory : IUpdateHandlerFactory
{
    private readonly IEnumerable<IUpdateHandlerResolver> _resolvers;

    public HandlerFactory(IEnumerable<IUpdateHandlerResolver> resolvers)
    {
        _resolvers = resolvers;
    }

    public IHandler CreateHandler(InteractionState interactionState, HandlerContext handlerContext)
    {
        var resolver = _resolvers.FirstOrDefault(r => r.CanHandle(interactionState));

        // TODO: UseCase do usuário enviar mensagem após estar no menu gerará erro
        //       Avaliar lançamento de exceção para tal UseCase

        return resolver!.ResolveHandler(handlerContext);
    }
}