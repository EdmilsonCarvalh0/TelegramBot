using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers.Contracts;
using TelegramBot.Application.Handlers.Interface;
using TelegramBot.Application.Handlers.ListViewAccess;
using TelegramBot.Application.Handlers.ListViewAccess.Utils;
namespace TelegramBot.Application.Handlers.ListViewAcess
{
    public class ListViewAccessHandler : Handler, IHandler
    {
        private readonly DateSelectionCache _cache;
        public ListViewAccessHandler(HandlerContext handlerContext) : base(handlerContext)
        {
            _cache = new();
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
                case ListViewAccessState.PresentOptionsFromExistingLists:
                    HandlePresentationOfExistingLists();
                    break;
                case ListViewAccessState.PresentChosenList:
                    HandleWithPreparationOfChosenList();
                    break;
            }
        }

        private void HandlePresentationOfExistingLists()
        {
            var dates = _handlerContext.ItemRepository.GetAllTheDates();

            _cache.RegisterDates(dates);

            _responseInfo.KeyboardMarkup = _handlerContext.KeyboardFactory.Create(dates, 2);
            _responseInfo.Subject = ListViewAccessState.PresentOptionsFromExistingLists;

            SetNewStateFlow(ListViewAccessState.PresentChosenList);
        }

        private void HandleWithPreparationOfChosenList()
        {
            var callbackData = _handlerContext.Context!.CallbackQuery!.Data!;

            var list = GetListInRepositoryToPresent(callbackData);

            _responseInfo.SubjectContextData = $"{callbackData}\n\n{list}";
            _responseInfo.Subject = ListViewAccessState.PresentChosenList;
        }

        private string GetListInRepositoryToPresent(string callbackData)
        {
            var infoChosenDate = HandleDataContainedInCallbackButton(callbackData);
            var date = _cache.GetDataInCollection(infoChosenDate);

            var items = _handlerContext.ItemRepository.GetListOfItems(date);

            return items.ToString();
        }

        private List<string> HandleDataContainedInCallbackButton(string callbackData)
        {
            return callbackData.Split([" de ", " às "], StringSplitOptions.None).ToList();
        }
    }
}
