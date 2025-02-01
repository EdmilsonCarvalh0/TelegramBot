using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Domain;
using TelegramBot.Service;

namespace TelegramBot.UserInterface;

//TODO: Create Class about List/Data for encapsulate functionalities
//      criar classe sobre list/dados para encapsular funcionalidades
public class ItemController : IItemController
{
    private IItemRepository _itemRepository;
    private ResponseContent _responseContent = new();
    private Dictionary<string, string> ResponseMessage { get; set; } = new();
    private Dictionary<string, string> ResponseCallback { get; set; } = new();

    public ItemController()
    {
        _itemRepository = new JsonItemRepository();
        LoadResponseMessages();
    }

    private void LoadResponseMessages()
    {
        ResponseMessage = new Dictionary<string, string>()
        {
            { "Initial Message", "Para ir para o menu inicial digite 'Menu'." },
            { "Menu", $"Olá, bem vindo ao Bot de Compras Mensais!\nEscolha uma opção:" },
            { "Item Added", $"Pronto, já adicionei!" },
            { "Update Item", "O que deseja alterar " },
            { "Non-existent Item", $"Infelizmente não encontrei o item na lista.\n\n" },
            { "Non-existent Item 2", "\n\nVerifique se o nome está correto e informe novamente." },
            { "Update Item OK", "Pronto, alterei pra você." },
            { "Deleted Item OK", "Item removido." },
            { "Created New List", "Nova lista criada." }
        };

        ResponseCallback = new Dictionary<string, string>()
        {
            { "Show List", $"Essa é a sua lista atual:\n\n" },
            { "Atualizar lista",  "Selecione o tipo de atualização que deseja fazer:" },
            { "Adicionar um item",  "Ótimo! Para fazer a inserção você pode enviar um ou mais itens se desejar.\n" + 
                                    "Lembrando que apenas o nome é obrigatório, mas é ideal que você informe a Marca e o Preço também para uma melhor experiência!\n\n" +
                                    "Vamos lá! Para inserir o item, siga a seguinte extrutura:\nProduto - Marca - Preco\nProduto - Marca - Preco\n\nAgora, por favor, informe os itens que deseja adicionar:" },
            { "Alterar um item",  "Por favor, informe agora o nome do item que deseja fazer alteração:" },
            { "Atributte Change",  "Informe " },
            { "Remover um item",  "Por favor, informe agora o nome do item que deseja remover:" },
            { "Criar nova lista",  "Envie agora os itens da nova lista.\nUse o padrão adequado para registro correto dos itens. Nesse caso, separe os itens por vírgula sem espaços.\nEx: Pão,Manteiga,Queijo" }
        };

    }

    public ResponseContent GetResponseCallback(string subject)
    {
        _responseContent.ResetResponseContent();

        _responseContent.Text = ResponseCallback[subject];
        return _responseContent;
    }

    public ResponseContent GetResponseMessage(string subject)
    {
        _responseContent.ResetResponseContent();

        _responseContent.Text = ResponseMessage[subject];
        return _responseContent;
    }

    public ResponseContent GetInitialMessage()
    {   
        _responseContent.ResetResponseContent();

        _responseContent.Text = ResponseMessage["Initial Message"];
        _responseContent.UserState = UserState.None;
        return _responseContent;
    }

    public ResponseContent StartService()
    {
        _responseContent.ResetResponseContent();

        _responseContent.Text = ResponseMessage["Menu"];
        _responseContent.UserState = UserState.Running;
        _responseContent.KeyboardMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Ver lista"),
                    InlineKeyboardButton.WithCallbackData("Atualizar lista"),
                    InlineKeyboardButton.WithCallbackData("Criar nova lista")
                }
            }
        );

        return _responseContent;
    }

    public ResponseContent GetOptionsOfListUpdate(string callbackMessage)
    {
        _responseContent.ResetResponseContent();

        _responseContent.Text = ResponseCallback[callbackMessage];
        _responseContent.KeyboardMarkup =  new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Adicionar um item"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Alterar um item")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Remover um item")
                }
            }
        );

        return _responseContent;
    }

    public ResponseContent GetAttributeOptions()
    {
        _responseContent.ResetResponseContent();

        _responseContent.Text = ResponseMessage["Update Item"];
        _responseContent.UserState = UserState.UpdateItem;
        _responseContent.KeyboardMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Nome"),
                    InlineKeyboardButton.WithCallbackData("Marca"),
                    InlineKeyboardButton.WithCallbackData("Preço")
                }
            }
        );

        return _responseContent;
    }

    public ResponseContent CheckItemExistence(string nameAttribute)
    {
        _responseContent.ResetResponseContent();

        var response = _itemRepository.GetItemInRepository(nameAttribute);

        if(response.Equals("item não encontrado.", StringComparison.CurrentCultureIgnoreCase))
        {
            _responseContent.Text = ResponseMessage["Non-existent Item"] 
                                    + _itemRepository.GetList()
                                    + ResponseMessage["Non-existent Item 2"];

            _responseContent.UserState = UserState.UpdateList;
            //_responseContent.AdditionalResponseContext = "Item no exist";
            return _responseContent;
        }

        if(response.Contains('\n'))
        {
            _responseContent.Text = $"Encontrei os seguintes itens:\n\n{response}\nQual deles você quer alterar?";
            _responseContent.UserState = UserState.UpdateList;
            return _responseContent;
        }

        _responseContent.UserState = UserState.UpdateItem;
        return _responseContent;
    }

    public ResponseContent AddItemInShoppingData(string userItems)
    {
        _responseContent.ResetResponseContent();

        _itemRepository.AddItemInList(userItems);
        _responseContent.Text = ResponseMessage["Item Added"];
        return _responseContent;
    }

    public ResponseContent SendItemToUpdateList(string item)
    {
        _responseContent.ResetResponseContent();

        _itemRepository.UpdateList(item);
        _responseContent.Text = ResponseMessage["Update Item OK"];
        _responseContent.UserState = UserState.None;
        return _responseContent;
    }

    public ResponseContent SendItemToRemoveFromList(string item)
    {
        _responseContent.ResetResponseContent();

        _itemRepository.RemoveItemFromList(item);
        _responseContent.Text = ResponseMessage["Deleted Item OK"];
        _responseContent.UserState = UserState.None;
        return _responseContent;
    }
    
    public ResponseContent ShowList()
    {
        _responseContent.ResetResponseContent();

        _responseContent.Text = ResponseCallback["Show List"] + _itemRepository.GetList();
        _responseContent.UserState = UserState.None;
        return _responseContent;
    }

    public ResponseContent GetItemsToCreatelist(string items)
    {
        _responseContent.ResetResponseContent();

        //TODO: manipulate TimeStamp to formalize data
        _itemRepository.CreateNewList(items);
        _responseContent.Text = ResponseMessage["Created New List"];
        _responseContent.UserState = UserState.None;
        return _responseContent;
    }

    //TODO: implement verify function automatic of item
}