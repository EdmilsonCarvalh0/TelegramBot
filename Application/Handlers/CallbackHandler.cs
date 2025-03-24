
using TelegramBot.Domain;

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
        }

        return _responseInfo!;
    }

    private void HandleSendOfList()
    {
        _responseInfo.SubjectContextData = _handlerContext.ItemRepository.GetList();
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

    }

    private void HandleAttributeChange()
    {

    }

    private void HandleItemRemove()
    {
        _responseInfo.Subject = "Remover um item";

        _handlerContext.StateManager.SetAdditionalInfo(_handlerContext.Context!.UserId, "waiting_item_to_remove");
    }

    private void HandleCreatingNewList()
    {

    }
}