using Application.Handlers;
using Application.Handlers.Interface;
using Telegram.Bot.Types;
using TelegramBot.Infrastructure;

namespace TelegramBot.Application.Handlers
{
    public class UpdateHandlers
    {
        private readonly HandlerContext _handlerContext;
        private readonly IResponseManager _responseManager;
        

        public UpdateHandlers(IResponseManager responseManager, HandlerContext handlerContext)
        {
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
            IUpdateHandlerFactory handlerFactory = new UpdateHandlerFactory();
            var handler = handlerFactory.CreateHandler(update, _handlerContext);

            var responseInfo = handler.Handle();

            _responseManager.ProcessResponse(responseInfo, _handlerContext.Context!.CancellationToken);

            return Task.CompletedTask;
        }
    }
}
