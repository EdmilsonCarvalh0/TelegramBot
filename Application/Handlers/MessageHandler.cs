
using TelegramBot.Domain;

namespace TelegramBot.Application;

public class MessageHandler
{
    private readonly HandlerContext _handlerContext;
    private readonly ResponseInfoToSendToTheUser _responseInfo = new();

    public MessageHandler(HandlerContext handlerContext)
    {
        _handlerContext = handlerContext;
    }

    public ResponseInfoToSendToTheUser Handle()
    {
        _handlerContext.StateManager.ShowUserState(_handlerContext.Context!.UserId);

        ProcessMenuRequest();

        if (_responseInfo == null) return _responseInfo!;

        var userStateData = _handlerContext.StateManager.GetUserStateData(_handlerContext.Context!.UserId);

        switch (userStateData.AdditionalInfo)
        {
            case "waiting_item_to_add":
                HandleWaitingToAddOfItem();
                break;
                
            case "waiting_for_attribute_to_update":
                HandleWaitingForAttributeToUpdated();
                break;

            case "waiting_for_name_attribute_to_update":
                HandleWaitingForNameAttributeToUpdated();
                break;
                
            case "waiting_item_to_remove":
                HandleWaitingItemToRemove();
                break;

            case "waiting_items_to_create_new_list":
                HandleWaitingItemsCreatingNewList();
                break;
        }

        return _responseInfo;
    }

    private void ProcessMenuRequest()
    {
        var context = _handlerContext.Context!;
        var stateManager = _handlerContext.StateManager!;
        var userState = stateManager.GetUserStateData(context.UserId);

        if (userState.State == UserState.None)
        {
            string messageContent = context.Message!.Text!;

            bool isMenu = messageContent.Equals("menu", StringComparison.CurrentCultureIgnoreCase);

            _responseInfo.Subject = isMenu ? char.ToUpper(messageContent[0]) + messageContent[1..].ToLower() : "Initial Message";
        }
    }

    private void HandleWaitingToAddOfItem()
    {
         
    }

    private void HandleWaitingForAttributeToUpdated()
    {

    }

    private void HandleWaitingForNameAttributeToUpdated()
    {

    }

    private void HandleWaitingItemToRemove()
    {

    }

    private void HandleWaitingItemsCreatingNewList()
    {

    }
}