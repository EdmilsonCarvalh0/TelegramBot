using Telegram.Bot.Types;
using TelegramBot.Application;

namespace Application.Handlers.Interface;

public interface IUpdateHandlers
{
    ResponseInfoToSendToTheUser Handle();
}