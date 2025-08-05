using TelegramBot.Application;
using TelegramBot.Application.Bot;
using TelegramBot.Application.DTOs;

public interface IResponseManager
{
    ResponseContent ProcessResponse(ResponseInfoToSendToTheUser reponseInfo);
}