
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

    }

    private void HandleToUpdateList()
    {

    }

    private void HandleToAddItem()
    {

    }

    private void HandleItemChange()
    {

    }

    private void HandleAttributeChange()
    {

    }

    private void HandleItemRemove()
    {
        
    }

    private void HandleCreatingNewList()
    {

    }
}