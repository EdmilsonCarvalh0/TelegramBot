using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Application;

namespace Application.Handlers.Interface;

public interface IUpdateHandlerFactory
{
    IUpdateHandlers CreateHandler(Update update, HandlerContext handlerContext);
}