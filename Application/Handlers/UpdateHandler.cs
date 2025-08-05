using Application.Handlers.Interface;
using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers.Interface;
using TelegramBot.Infrastructure;

namespace TelegramBot.Application.Handlers;

public class UpdateHandler : IUpdateHandlers
{
    private readonly HandlerContext _handlerContext;
    private readonly IUpdateHandlerFactory _factory;

    public UpdateHandler(HandlerContext handlerContext, IUpdateHandlerFactory factory)
    {
        _handlerContext = handlerContext;
        _factory = factory;
    }

    // TODO: Avaliar o uso do StateManager no contexto da classe
    // Alta Coesão?
    // public void Start(BotRequestContext context)
    // {

    //     if (!_handlerContext.StateManager!.ContainsUserId(_handlerContext.Context!.UserId))
    //     {
    //         _handlerContext.StateManager.SetUserId(_handlerContext.Context.UserId);
    //     }
    // }

    public ResponseInfoToSendToTheUser DelegateUpdates(BotRequestContext context)
    {
        _handlerContext.Context = context;
        var userId = _handlerContext.Context!.UserId;

        // AVALIAR CRIAÇÃO DE UMA CLASSE QUE SEJA RESPONSÁVEL POR DEFINIR INICIALMENTE CADA STATE
        _handlerContext.StateManager.CheckIfContainsUserId(context!.UserId);
        _handlerContext.StateManager.CheckDataButton(userId, context.CallbackQuery!);

        var handler = _factory.CreateHandler(_handlerContext.StateManager.GetInteractionState(userId), _handlerContext);

        return handler.Handle();
    }


    // TODO: Avaliar contexto da função de acordo com a coes�o da classe
    // private void CheckDataButton(Update update, long userId)
    // {
    //     if (update.CallbackQuery != null && update.CallbackQuery!.Data == "Ver listas")
    //     {
    //         UserStateManager.SetState(userId, InteractionState.ListViewEnabled);
    //     }

    //     if (update.CallbackQuery != null && update.CallbackQuery!.Data == "Modo Ficando Mais Pobre")
    //     {
    //         UserStateManager.SetState(userId, InteractionState.ShoppingModeEnabled);
    //     }
    // }
}
