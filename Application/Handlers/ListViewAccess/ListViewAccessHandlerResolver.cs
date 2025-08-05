using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers.Interface;
using TelegramBot.Application.Handlers.ListViewAcess;

namespace TelegramBot.Application.Handlers.ListViewAccess
{
    public class ListViewAcsessHandlerResolver : IUpdateHandlerResolver
    {
        public bool CanHandle(InteractionState interactionState) => interactionState == InteractionState.ListViewEnabled;

        public IHandler ResolveHandler(HandlerContext context) => new ListViewAccessHandler(context);
    }
}
