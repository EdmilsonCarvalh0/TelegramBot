using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure;

namespace TelegramBot.Application
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
            ResponseInfoToSendToTheUser responseInfo = new ResponseInfoToSendToTheUser();

            if (update.Type == UpdateType.Message && update.Message != null)
            {
                MessageHandler messageHandler = new MessageHandler(_handlerContext);
                responseInfo = messageHandler.Handle();
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                CallbackHandler callbackHandler = new CallbackHandler(_handlerContext);
                responseInfo = callbackHandler.Handle();
            }

            _responseManager.ProcessResponse(responseInfo, _handlerContext.Context!.CancellationToken);

            return Task.CompletedTask;
        }
    }
}
