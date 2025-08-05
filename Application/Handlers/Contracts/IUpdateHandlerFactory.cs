using Telegram.Bot.Types;
using TelegramBot.Application.DTOs;

namespace TelegramBot.Application.Handlers.Interface;

public interface IUpdateHandlerFactory
{
    IHandler CreateHandler(InteractionState interactionState, HandlerContext handlerContext);
}