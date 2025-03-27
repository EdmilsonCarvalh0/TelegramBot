
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

            case "waiting_for_number_that_references_to_update":
                HandleNumberReferencingItem();
                break;

            case "waiting_for_number_that_references_to_remove":
                HandleNumberReferencingItem();
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
        _handlerContext.ItemRepository.AddItemInList(_handlerContext.Context!.Message!.Text!);
        _handlerContext.StateManager.ResetAdditionalInfo(_handlerContext.Context!.UserId);
        
        _responseInfo.Subject = "Item Added";
    }

    private void HandleWaitingForAttributeToUpdated()
    {
        var attribute = _handlerContext.Context!.Message!.Text!;
        _handlerContext.ItemRepository.UpdateItemInList(attribute);
        _handlerContext.StateManager.ResetAdditionalInfo(_handlerContext.Context!.UserId);

        _responseInfo.Subject = "Update Item OK";
    }

    private void HandleWaitingForNameAttributeToUpdated()
    {
        var nameAttribute = _handlerContext.Context!.Message!.Text!;
        var searchResult = _handlerContext.ItemRepository.GetItemInRepository(nameAttribute!);

        if (searchResult.Status == SearchStatus.NotFound)
        {
            _responseInfo.Subject = "Non-existent Item";
            _responseInfo.SubjectContextData = _handlerContext.ItemRepository.GetList();
            return;
        }

        if (searchResult.Status == SearchStatus.MoreThanOne)
        {
            _responseInfo.Subject = "More Than One Item";
            _responseInfo.SubjectContextData = "";
            _responseInfo.SubjectContextData = searchResult.Result;
            _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_for_number_that_references_to_update");
            return;
        }

        var genderVerified = CheckAttributeGender(nameAttribute);
        _responseInfo.SubjectContextData = $"{genderVerified} {nameAttribute}";

        _handlerContext.ItemRepository.AddItemInEditingArea(nameAttribute!);
        _responseInfo.Subject = "Update Item";
    }

    private void HandleWaitingItemToRemove()
    {
        var result = _handlerContext.ItemRepository.RemoveItemFromList(_handlerContext.Context!.Message!.Text!);

        if (result.Status == SearchStatus.NotFound)
        {
            _responseInfo.Subject = "Non-existent item";
            _responseInfo.SubjectContextData = _handlerContext.ItemRepository.GetList();
            return;
        }

        if (result.Status == SearchStatus.MoreThanOne)
        {
            _responseInfo.Subject = "More Than One Item";
            _responseInfo.SubjectContextData = "";
            _responseInfo.SubjectContextData = result.Result;
            _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_for_number_that_references_to_remove");
            return;
        }

        _handlerContext.StateManager.ResetAdditionalInfo(_handlerContext.Context.UserId);
        _responseInfo.Subject = "Deleted Item OK";
    }

    private void HandleNumberReferencingItem()
    {
        var userId = _handlerContext.Context!.UserId;
        var stateData = _handlerContext.StateManager.GetUserStateData(userId);
        var inputNumber = _handlerContext.Context!.Message!.Text!;

        bool isNumber = CheckIfItIsANumber(inputNumber);
        int referenceNumber = 0;

        if(isNumber)
        {
            referenceNumber = Convert.ToInt32(inputNumber);
        } 
        
        if(!isNumber)
        {
            _responseInfo.Subject = "Invalid Number";
            return;
        }

        UserState response = _handlerContext.ItemRepository.VerifyNumberReferencingItem(referenceNumber, stateData.AdditionalInfo);

        switch (response)
        {
            case UserState.None:
                _responseInfo.Subject = "Invalid Number";
                break;

            case UserState.UpdateItem:
                _responseInfo.Subject = "Update Item";
                break;

            case UserState.DeleteItem:
                _responseInfo.Subject = "Deleted Item OK";
                _handlerContext.StateManager.ResetAdditionalInfo(_handlerContext.Context!.UserId);
                break;
        }
    }

    private void HandleWaitingItemsCreatingNewList()
    {

    }

    private string CheckAttributeGender(string item)
    {
        return item[item.Length - 1] == 'a' ? "da " : "do ";
    }

    private bool CheckIfItIsANumber(string inputNumber)
    {
        return inputNumber.Length == 1 && int.TryParse(inputNumber, out _);
    }
}