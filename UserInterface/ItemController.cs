using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Domain;
using TelegramBot.Service;

namespace TelegramBot.UserInterface;

//TODO: Create Class about List/Data for encapsulate functionalities
//      criar classe sobre list/dados para encapsular funcionalidades
public class ItemController : IItemController
{
    private static readonly IItemRepository _itemRepository = new JsonItemRepository();
    private ResponseContent _responseContent = new();
    private Dictionary<string, string> ResponseMessage { get; set; }

    public ItemController()
    {
        ResponseMessage = new Dictionary<string, string>()
        { 
            { "Initial Message", "Para ir para o menu inicial digite 'Menu'." },
            { "Menu", $"Olá, bem vindo ao Bot de Compras Mensais!\nEscolha uma opção:" },
            { "Item Added", $"Pronto, já adicionei!" },
            { "Update Item", "O que deseja alterar " },
            { "Non-existent Item", $"Infelizmente não encontrei o item na lista.\\n\\n{ShowList()}\\n\\nVerifique se o nome está correto e informe novamente.\"" },
            { "Update Item OK", "Pronto, alterei pra você." }
        };
    }

    public ResponseContent GetInitialMessage()
    {
        _responseContent.Text = ResponseMessage["Initial Message"];
        _responseContent.UserState = UserState.None;
        return _responseContent;
    }

    public ResponseContent StartService()
    {
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

    public ResponseContent GetOptionsOfListUpdate()
    {
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
        var response = _itemRepository.GetItemInRepository(nameAttribute);

        if(response.Equals("item não encontrado.", StringComparison.CurrentCultureIgnoreCase))
        {
            _responseContent.Text = ResponseMessage["Non-existent Item"];
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
        _itemRepository.AddItemInList(userItems);
        _responseContent.Text = ResponseMessage["Item Added"];
        return _responseContent;
    }

    public ResponseContent SendItemToUpdateList(string item)
    {
        _itemRepository.UpdateList(item);
        _responseContent.Text = ResponseMessage["Update Item OK"];
        _responseContent.UserState = UserState.None;
        return _responseContent;
    }

    public string SendItemToRemoveFromList(string item)
    {
        _itemRepository.RemoveItemFromList(item);
        return "Item removido.";
    }
    
    public string ShowList()
    {
        return $"Essa é a sua lista atual:\n\n{_itemRepository.GetList()}";
    }

    public string GetItemsToCreatelist(string items)
    {
        //TODO: manipulate TimeStamp to formalize data
        _itemRepository.CreateNewList(items);
        return "Nova lista criada.";
    }

    //TODO: implement verify function automatic of item
}