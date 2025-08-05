using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers.Interface;
using TelegramBot.DataModels.Item.Snapshot;
using TelegramBot.UserInterface;
using TelegramBot.UserInterface.Interfaces;

namespace TelegramBot.Application.Handlers;

public class CallbackHandler : IHandler
{
    private readonly HandlerContext _handlerContext;
    private readonly ResponseInfoToSendToTheUser _responseInfo = new();

    public CallbackHandler(HandlerContext handlerContext)
    {
        _handlerContext = handlerContext;
    }

    public ResponseInfoToSendToTheUser Handle()
    {
        var context = _handlerContext.Context!;

        _handlerContext.StateManager!.ShowUserState(context!.UserId);

        // TODO: Precisa ser revisado para ser inserido no switch
        if (_handlerContext.StateManager.GetUserStateData(context.UserId).AdditionalInfo == "waiting_for_the_date_of_the_chosen_list")
        {
            HandleSendOfList();
            _handlerContext.StateManager.ResetAdditionalInfo(context.UserId);
            return _responseInfo;
        }

        switch (context.CallbackQuery!.Data)
        {
            case "Ver listas":
                HandleWithThePresentationOfExistingLists();
                break;
            
            case "waiting_for_the_date_of_the_chosen_list":
                HandleSendOfList();
                break;
            // Revisar \/
            case "waiting_for_number_that_references_to_update":
                HandleSendOfList();
                break;
            
            //case "Atualizar lista":
            //    HandleToUpdateList();
            //    break;

            //case "Adicionar um item":
            //    HandleToAddItem();
            //    break;

            //case "Alterar um item":
            //    HandleItemChange();
            //    break;

            //case "Nome":
            //case "Marca":
            //case "Preço":
            //    HandleAttributeChange();
            //    break;

            //case "Remover um item":
            //    HandleItemRemove();
            //    break;
            
            //case "Criar nova lista":
            //    HandleCreatingNewList();
            //    break;
            
            case "Modo Ficando Mais Pobre":
                HandleStartOfShoppingMode();
                break;
            
            case "Sim":
            case "Não":
                HandleShoppingModeConfirmation();
                break;

            case "Mais uma coisa":
                HandleWithUnlistedItem();
                break;
            
            case "Só errei o nome":
                HandleWrongItemName();
                break;
        }

        return _responseInfo!;
    }

    private void HandleSendOfList()
    {
        var callbackData = _handlerContext.Context!.CallbackQuery!.Data!;
        
        // TODO: Encapsular lógica de tratamento de Date em uma function
        List<string> info = callbackData.Split([" de ", " às "], StringSplitOptions.None).ToList();

        // TODO: Refatorar e remover a dependência da classe concreta
        ShoppingDateTime shoppingDateTime = new ShoppingDateTime(info[1].ToLower(), Convert.ToInt32(info[0]), info[2]);
        
        var items = _handlerContext.ItemRepository.GetListOfItems(shoppingDateTime);

        // TODO: Implementar classe que trate de preparação de itens para exibição
        //       camada UserInterface?
        string list = string.Empty;

        foreach(var item in items.Values)
        {
            list += item.ToString();
        }
        
        _responseInfo.SubjectContextData = $"{callbackData}\n\n{list}";
        _responseInfo.Subject = "Show List";
    }

    private void HandleWithThePresentationOfExistingLists()
    {
        var dates = _handlerContext.ItemRepository.GetAllTheDates();

        _responseInfo.KeyboardMarkup = _handlerContext.KeyboardFactory.Create(dates, 2);

        _responseInfo.Subject = "Choose List";
        
        _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_for_the_date_of_the_chosen_list");
    }

    private void HandleToUpdateList()
    {
        _responseInfo.Subject = "Atualizar lista";
    }

    private void HandleToAddItem()
    {
        _responseInfo.Subject = "Adicionar um item";
        _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_item_to_add");
    }

    private void HandleItemChange()
    {
        _responseInfo.Subject = _handlerContext.Context!.CallbackQuery!.Data;
        _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_for_name_attribute_to_update");
    }

    private void HandleAttributeChange()
    {
        var nameAttribute = _handlerContext.Context!.CallbackQuery!.Data!;
        _handlerContext.ItemRepository.AddAttributeToBeChangedInEditingArea(nameAttribute!);

        var genderVerified = CheckItemGender(nameAttribute);
        _responseInfo.SubjectContextData = $"{genderVerified} {nameAttribute}";
        _responseInfo.Subject = "Attribute Change";

        _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_for_attribute_to_update");
    }

    private void HandleItemRemove()
    {
        _responseInfo.Subject = "Remover um item";
        _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_item_to_remove");
    }

    private void HandleStartOfShoppingMode()
    {
        _responseInfo.Subject = "Modo Ficando Mais Pobre";
    }

    private void HandleShoppingModeConfirmation()
    {
        if(_handlerContext.Context!.CallbackQuery!.Data! == "Sim")
        {
            _responseInfo.Subject = "Assistant List Items";
            _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_for_the_assistant_list_items");
        }

        if(_handlerContext.Context!.CallbackQuery!.Data! == "Não")
        {
            _responseInfo.Subject = "Shopping Mode Declined";
        }
    }

    private void HandleWithUnlistedItem()
    {
        _handlerContext.ShoppingAssistant.SaveNewItem();
        _responseInfo.Subject = "Inserting New Item";

        var itemName = _handlerContext.ShoppingAssistant.GetItemReserved();
        string genderVerified = CheckItemGender(itemName.Name);

        _responseInfo.SubjectContextData = $"{genderVerified} {itemName.Name}";
    }

    private void HandleWrongItemName()
    {
        _responseInfo.Subject = "Discard Wrong Item";
    }

    private string CheckItemGender(string item)
    {
        return item[item.Length -1] == 'a' ? "a nova" : "o novo";
    }
}