using Microsoft.AspNetCore.OutputCaching;
using Telegram.Bot.Types;
using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers.Contracts;
using TelegramBot.Application.Handlers.Interface;

namespace TelegramBot.Application.Handlers.InitialMessage;

public class InitialMessageHandler : Handler, IHandler
{
    public InitialMessageHandler(HandlerContext handlerContext) : base(handlerContext) { }

    public ResponseInfoToSendToTheUser Handle()
    {
        CheckIfMessageIsNull(_handlerContext.Context!.Message!);

        PrepareNewStateFlowIfIsMenu();

        var state = PrepareToDelegate();

        Delegate(state);

        return _responseInfo;
    }

    protected override void Delegate(Enum state)
    {
        _responseInfo.Subject = state;
    }

    private void CheckIfMessageIsNull(Message messageContent)
    {
        ArgumentNullException.ThrowIfNull(messageContent);
    }

    private void PrepareNewStateFlowIfIsMenu()
    {
        if (CheckIfMessageIsMenu())
        {
            SetNewStateFlow(InitialMessageState.PresentMenu);
        }
    }

    private bool CheckIfMessageIsMenu()
    {
        return _handlerContext.Context!.Message!.Text!.Equals("menu", StringComparison.CurrentCultureIgnoreCase);
    }
}
