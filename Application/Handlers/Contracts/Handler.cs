using TelegramBot.Application.DTOs;

namespace TelegramBot.Application.Handlers.Contracts;

public abstract class Handler
{
    protected readonly HandlerContext _handlerContext;
    protected readonly ResponseInfoToSendToTheUser _responseInfo;

    public Handler(HandlerContext handlerContext)
    {
        _handlerContext = handlerContext;
        _responseInfo = new();
    }

    protected Enum PrepareToDelegate()
    {
        var userId = _handlerContext.Context!.UserId;
        return _handlerContext.StateManager.GetStateFlow(userId);
    }

    protected abstract void Delegate(Enum state);

    protected void SetNewStateFlow(Enum state)
    {
        var userId = _handlerContext.Context!.UserId;
        _handlerContext.StateManager.SetStateFlow(userId, state);
    }
}
