using Telegram.Bot.Types;
using TelegramBot.Application;
using TelegramBot.Application.DTOs;
using TelegramBot.Infrastructure;

namespace Application.Handlers.Interface;

public interface IUpdateHandlers
{
    ResponseInfoToSendToTheUser DelegateUpdates(BotRequestContext context);
}