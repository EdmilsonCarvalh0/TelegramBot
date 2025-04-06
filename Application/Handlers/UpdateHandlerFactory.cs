using Application.Handlers.Interface;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application;
using TelegramBot.Infrastructure;

namespace Application.Handlers;

public class UpdateHandlerFactory : IUpdateHandlerFactory
{
    public IUpdateHandlers CreateHandler(Update update, HandlerContext handlerContext)
    {
        if (update.Type == UpdateType.Message && update.Message != null)
        {
            return new MessageHandler(handlerContext);
        }
        
        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
        {
            return new CallbackHandler(handlerContext);
        }

        throw new NotSupportedException("Tipo de update não suportado.");
    }
}