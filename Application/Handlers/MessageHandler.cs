using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers.Interface;
using TelegramBot.DataModels.Item.Snapshot;
using TelegramBot.Domain;
using TelegramBot.Domain.Item.Input;

namespace TelegramBot.Application.Handlers;

public class MessageHandler : IHandler
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
            case "waiting_for_number_that_references_to_remove":
                HandleNumberReferencingItem();
                break;

            case "waiting_item_to_remove":
                HandleWaitingItemToRemove();
                break;

            case "waiting_items_to_create_new_list":
                HandleWaitingItemsCreatingNewList();
                break;

            case "waiting_for_the_assistant_list_items":
                HandleWithTheListOfItemsToBeBuiyng();
                break;

            case "waiting_for_the_item_added_to_the_cart":
                HandleAssistantListItems();
                break;
        }

        return _responseInfo;
    }

    private void ProcessMenuRequest()
    {
        var context = _handlerContext.Context!;
        var stateManager = _handlerContext.StateManager;
        var userState = stateManager.GetUserStateData(context.UserId);

        if (userState.State == InteractionState.None)
        {
            string messageContent = context.Message!.Text!;

            bool isMenu = messageContent.Equals("menu", StringComparison.CurrentCultureIgnoreCase);

            _responseInfo.Subject = isMenu ? char.ToUpper(messageContent[0]) + messageContent[1..].ToLower() : "Initial Message";
        }
    }

    private void HandleWaitingToAddOfItem()
    {
        var inputItems = ItemInputService.ProcessRawInput(_handlerContext.Context!.Message!.Text!);
        _handlerContext.ItemRepository.AddItemInList(inputItems);
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
        var searchResult = _handlerContext.ItemRepository.GetItemInRepository(nameAttribute);

        if (searchResult.Status == SearchStatus.NotFound)
        {
            _responseInfo.Subject = "Non-existent Item";
            _responseInfo.SubjectContextData = ProcessListInRepositoryToShow();
            return;
        }

        if (searchResult.Status == SearchStatus.MoreThanOne)
        {
            _responseInfo.Subject = "More Than One Item";
            _responseInfo.SubjectContextData = searchResult.Result;
            _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_for_number_that_references_to_update");
            return;
        }

        var genderVerified = CheckAttributeGender(nameAttribute);
        _responseInfo.SubjectContextData = $"{genderVerified} {nameAttribute}";

        _handlerContext.ItemRepository.AddItemInEditingArea(nameAttribute);
        _responseInfo.Subject = "Update Item";
    }

    private void HandleWaitingItemToRemove()
    {
        var result = _handlerContext.ItemRepository.RemoveItemFromList(_handlerContext.Context!.Message!.Text!);

        if (result.Status == SearchStatus.NotFound)
        {
            _responseInfo.Subject = "Non-existent item";
            _responseInfo.SubjectContextData = ProcessListInRepositoryToShow();
            return;
        }

        if (result.Status == SearchStatus.MoreThanOne)
        {
            _responseInfo.Subject = "More Than One Item";
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

        if(!isNumber)
        {
            _responseInfo.Subject = "Invalid Number";
            return;
        }

        int referenceNumber = Convert.ToInt32(inputNumber);

        var response = _handlerContext.ItemRepository.VerifyNumberReferencingItem(referenceNumber, stateData.AdditionalInfo);

        switch (response)
        {
            case InteractionState.None:
                _responseInfo.Subject = "Invalid Number";
                break;

            case InteractionState.UpdateItem:
                _responseInfo.Subject = "Update Item";
                break;

            case InteractionState.DeleteItem:
                _responseInfo.Subject = "Deleted Item OK";
                _handlerContext.StateManager.ResetAdditionalInfo(_handlerContext.Context!.UserId);
                break;
        }
    }

    private void HandleWaitingItemsCreatingNewList()
    {

    }

    private void HandleAssistantListItems()
    {
        var inputItems = ItemInputService.ProcessRawInput(_handlerContext.Context!.Message!.Text!);
        
        var wasRemoved = _handlerContext.ShoppingAssistant.RemoveItemFromShoppingList(inputItems[0]);
        if (!wasRemoved)
        {
            _responseInfo.Subject = "Item Not Listed";
            _handlerContext.ShoppingAssistant.ReserveNewItem(inputItems[0]);
            return;
        }
        
        _handlerContext.ShoppingAssistant.AddInputItemToPurchasedList(inputItems);

        var total = _handlerContext.ShoppingAssistant.GetTotalPrice();
        
        var isEmpty = _handlerContext.ShoppingAssistant.CheckIfListIsEmpty();
        if (isEmpty)
        {
            _handlerContext.ShoppingAssistant.PrepareForCompletion();
            _handlerContext.ShoppingAssistant.FinalizeAndSaveList();
            
            _responseInfo.Subject = "Off Shopping";
            _responseInfo.SubjectContextData = $"{total:C2}";
            
            _handlerContext.StateManager.ResetAdditionalInfo(_handlerContext.Context!.UserId);
            return;
        }

        var list = ProcessListInShoppingToShow();
        _responseInfo.Subject = "On Shopping";
        _responseInfo.SubjectContextData = list + $"\n\nTotal: {total:C2}";
    }

    private void HandleWithTheListOfItemsToBeBuiyng()
    {
        var items = _handlerContext.Context!.Message!.Text!;
        List<string> listItems = [.. items.Trim().Split(", ")];

        _handlerContext.ShoppingAssistant.LoadList(listItems);
        _handlerContext.ShoppingAssistant.BeginShoppingSession();
        
        _responseInfo.Subject = "Prepared List";
        _responseInfo.SubjectContextData = ProcessListInShoppingToShow();

        _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_for_the_item_added_to_the_cart");
    }

    private string CheckAttributeGender(string item)
    {
        return item[item.Length - 1] == 'a' ? "da " : "do ";
    }

    private bool CheckIfItIsANumber(string inputNumber)
    {
        return int.TryParse(inputNumber, out _);
    }

    private string ProcessListInRepositoryToShow()
    {
        var items = _handlerContext.ItemRepository.GetListOfItems(new ShoppingDateTime("abril", 17, "09:32"));
        string list = string.Empty;

        foreach(var item in items)
        {
            list += item.ToString();
        }

        return list;
    }

    private string ProcessListInShoppingToShow()
    {
        var itemsToBuy = _handlerContext.ShoppingAssistant.GetListOfRemainingItems();
        string list = string.Empty;

        for(int i = 0; i < itemsToBuy.Count; i++)
        {
            list += $"  {i+1}. {char.ToUpper(itemsToBuy[i][0]) + itemsToBuy[i][1..].ToLower()}\n";
        }

        return list;
    }
}