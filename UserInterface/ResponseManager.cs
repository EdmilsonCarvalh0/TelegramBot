using TelegramBot.Data;
using TelegramBot.Application;
using TelegramBot.Infrastructure;

namespace TelegramBot.UserInterface;

public class ResponseManager : IResponseManager
{
    private readonly MessageSender _messageSender;
    private readonly BotResponse _botResponse;

    public ResponseManager(MessageSender messageSender, BotResponse botResponse)
    {
        _messageSender = messageSender;
        _botResponse = botResponse;
    }

    public async Task ProcessResponse(ResponseInfoToSendToTheUser responseInfo, CancellationToken cancellationToken)
    {
        var response = _botResponse.GetResponse(responseInfo.Subject!);

        response.Text = string.Format(response.Text, responseInfo.SubjectContextData);

        await _messageSender.SendMessageAsync(response, cancellationToken);
    }
}