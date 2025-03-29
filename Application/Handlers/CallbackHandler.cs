namespace TelegramBot.Application;

public class CallbackHandler
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

        switch (context.CallbackQuery!.Data)
        {
            case "Ver lista":
                HandleSendOfList();
                break;
                
            case "Atualizar lista":
                HandleToUpdateList();
                break;

            case "Adicionar um item":
                HandleToAddItem();
                break;

            case "Alterar um item":
                HandleItemChange();
                break;

            case "Nome":
                HandleAttributeChange();
                break;
                
            case "Marca":
                HandleAttributeChange();
                break;
                
            case "Preço":
                HandleAttributeChange();
                break;

            case "Remover um item":
                HandleItemRemove();
                break;
            
            case "Criar nova lista":
                HandleCreatingNewList();
                break;
            
            case "Modo Ficando Mais Pobre":
                HandleStartOfShoppingMode();
                break;
            
            case "Sim":
                HandleShoppingModeConfirmation();
                break;
            
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
        var items = _handlerContext.ItemRepository.GetList();
        
        string list = string.Empty;

        foreach(var item in items)
        {
            list += item.ToString();
        }

        _responseInfo.SubjectContextData = list;
        _responseInfo.Subject = "Show List";
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

    private void HandleCreatingNewList()
    {

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
        _responseInfo.Subject = "Inserting New Item";

        var itemName = _handlerContext.ShoppingAssistant.GetItemReserved();
        string genderVerified = CheckItemGender(itemName);

        _responseInfo.SubjectContextData = $"{genderVerified} {itemName}";
    }

    private void HandleWrongItemName()
    {
        _responseInfo.Subject = "Discard Wrong Item";
    }

    private string CheckItemGender(string item)
    {
        return item[item.Length -1] == 'a' ? "a" : "o";
    }
}