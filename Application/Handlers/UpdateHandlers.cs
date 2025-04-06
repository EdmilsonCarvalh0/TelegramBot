using Application.Handlers.Interface;
using Telegram.Bot.Types;
using TelegramBot.Infrastructure;

namespace TelegramBot.Application
{
    public class UpdateHandlers
    {
        private readonly IUpdateHandlerFactory _updateHandlerFactory;
        private readonly HandlerContext _handlerContext;
        private readonly IResponseManager _responseManager;
        

        public UpdateHandlers(IUpdateHandlerFactory updateHandlerFactory, IResponseManager responseManager, HandlerContext handlerContext)
        {
            _updateHandlerFactory = updateHandlerFactory;
            _handlerContext = handlerContext;
            _responseManager = responseManager;
        }

        public void Start(BotRequestContext context)
        {
            _handlerContext.Context = context;
                
            if (!_handlerContext.StateManager!.ContainsUserId(_handlerContext.Context.UserId))
            {
                _handlerContext.StateManager.SetUserId(_handlerContext.Context.UserId);
            }
        }

        public Task DelegateUpdates(Update update)
        {
            IUpdateHandlers handler = _updateHandlerFactory.CreateHandler(update, _handlerContext);

            ResponseInfoToSendToTheUser responseInfo = handler.Handle();

            _responseManager.ProcessResponse(responseInfo, _handlerContext.Context!.CancellationToken);

            return Task.CompletedTask;
        }
    }
}
