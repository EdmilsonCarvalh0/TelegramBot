using TelegramBot.Application;

public interface IResponseManager
{
    Task ProcessResponse(ResponseInfoToSendToTheUser reponseInfo, CancellationToken cancellationToken);
}