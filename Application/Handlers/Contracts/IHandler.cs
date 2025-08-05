using TelegramBot.Application.DTOs;

namespace TelegramBot.Application.Handlers.Interface
{
    public interface IHandler
    {
        ResponseInfoToSendToTheUser Handle();
    }
}
