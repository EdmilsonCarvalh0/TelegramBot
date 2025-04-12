using Application.Handlers.Interface;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application;

namespace Application.Handlers;

public class UpdateHandlerFactory : IUpdateHandlerFactory
{
    public IUpdateHandlers CreateHandler(Update update, HandlerContext handlerContext)
    {
        switch (update.Type)
        {
            case UpdateType.Message when update.Message != null:
                return new MessageHandler(handlerContext);
            case UpdateType.CallbackQuery when update.CallbackQuery != null:
                return new CallbackHandler(handlerContext);
            default:
                throw new NotSupportedException("Tipo de update não suportado.");
        }
    }
}