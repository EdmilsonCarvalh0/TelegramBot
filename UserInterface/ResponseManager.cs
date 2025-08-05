using TelegramBot.Application.Bot;
using TelegramBot.Application.DTOs;
using TelegramBot.Infrastructure;
using TelegramBot.Infrastructure.Telegram;

namespace TelegramBot.UserInterface;

public class ResponseManager : IResponseManager
{
    private readonly BotResponse _botResponse;

    public ResponseManager(BotResponse botResponse)
    {
        _botResponse = botResponse;
    }

    public ResponseContent ProcessResponse(ResponseInfoToSendToTheUser responseInfo)
    {
        var response = _botResponse.GetResponse(responseInfo.Subject!);

        if (responseInfo.KeyboardMarkup != null) response.KeyboardMarkup = responseInfo.KeyboardMarkup;
        
        response.Text = string.Format(response.Text, responseInfo.SubjectContextData);

        return response;
    }
}