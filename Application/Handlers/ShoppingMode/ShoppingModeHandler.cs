using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers.Contracts;
using TelegramBot.Application.Handlers.Interface;

namespace TelegramBot.Application.Handlers.ShoppingMode;

public class ShoppingModeHandler : Handler, IHandler
{
    // AVALIAR CRIAÇÃO DO CACHE MANAGER OU RETER O IHANDLER EM ALGUM LUGAR DEVIDO AO CONTEXTO DE SEU USO
    public readonly ShoppingAssistantMode ShoppingAssistant;
    public ShoppingModeHandler(HandlerContext handlerContext) : base(handlerContext)
    {

    }

    public ResponseInfoToSendToTheUser Handle()
    {
        var state = PrepareToDelegate();

        Delegate(state);

        return _responseInfo;
    }

    protected override void Delegate(Enum state)
    {
        switch (state)
        {
            case ShoppingModeState.PreparationOfShoppingMode:
                HandleWithPreparationOfShoppingMode();
                break;
            case ShoppingModeState.OnShoppingMode:
                HandleWithTheProvidedList();
                break;
        }
    }

    private void HandleWithPreparationOfShoppingMode()
    {
        _responseInfo.Subject = ShoppingModeState.PreparationOfShoppingMode;
    }

    private void HandleWithTheProvidedList()
    {
        var items = _handlerContext.Context!.Message!.Text!;
        List<string> listItems = [.. items.Trim().Split(", ")];

        _handlerContext.ShoppingAssistant.LoadList(listItems);
        _handlerContext.ShoppingAssistant.BeginShoppingSession();
        
        _responseInfo.Subject = ShoppingModeState.OnShoppingMode;
        _responseInfo.SubjectContextData = ProcessListInShoppingToShow();

        SetNewStateFlow(ShoppingModeState.OnShoppingMode);
    }
}