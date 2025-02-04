using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;
using TelegramBot.Domain;
using TelegramBot.Service;

namespace TelegramBot.UserInterface;

//TODO: Create Class about List/Data for encapsulate functionalities
//      criar classe sobre list/dados para encapsular funcionalidades
public class ItemController : IItemController
{
    private readonly IItemRepository _itemRepository;
    private BotResponse _botResponse;
    private readonly ResponseContent _responseContent = new();
    private Dictionary<string, string> ResponseMessage { get; set; } = new();
    private Dictionary<string, string> ResponseCallback { get; set; } = new();

    public ItemController()
    {
        _itemRepository = new JsonItemRepository();
        _botResponse = new BotResponse();
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
        // _responseContent.ResetResponseContent();
        // _responseContent.Text = ResponseCallback[subject];

        return _botResponse.GetResponse(subject);
    }

    public ResponseContent GetResponseMessage(string subject)
    {
        // _responseContent.ResetResponseContent();
        // _responseContent.Text = ResponseMessage[subject];

        return _botResponse.GetResponse(subject);
    }

    public ResponseContent GetInitialMessage(string subject)
    {   
        // _responseContent.ResetResponseContent();

        // _responseContent.Text = ResponseMessage[subject];
        // _responseContent.UserState = UserState.None;
        // return _responseContent;

        return _botResponse.GetResponse(subject);
    }

    public ResponseContent StartService(string request)
    {
        // _responseContent.ResetResponseContent();

        // _responseContent.Text = ResponseMessage["Menu"];
        // _responseContent.UserState = UserState.Running;
        // _responseContent.KeyboardMarkup = new InlineKeyboardMarkup(new[]
        //     {
        //         new[]
        //         {
        //             InlineKeyboardButton.WithCallbackData("Ver lista"),
        //             InlineKeyboardButton.WithCallbackData("Atualizar lista"),
        //             InlineKeyboardButton.WithCallbackData("Criar nova lista")
        //         }
        //     }
        // );

        // return _responseContent;

        return _botResponse.GetResponse(request);
    }

    public ResponseContent GetOptionsOfListUpdate(string request)
    {
        // _responseContent.ResetResponseContent();

        // _responseContent.Text = ResponseCallback[callbackMessage];
        // _responseContent.KeyboardMarkup =  new InlineKeyboardMarkup(new[]
        //     {
        //         new []
        //         {
        //             InlineKeyboardButton.WithCallbackData("Adicionar um item"),
        //         },
        //         new[]
        //         {
        //             InlineKeyboardButton.WithCallbackData("Alterar um item")
        //         },
        //         new[]
        //         {
        //             InlineKeyboardButton.WithCallbackData("Remover um item")
        //         }
        //     }
        // );

        // return _responseContent;

        return _botResponse.GetResponse(request);
    }

    public ResponseContent GetAttributeOptions(string request)
    {
        // _responseContent.ResetResponseContent();

        // _responseContent.Text = ResponseMessage["Update Item"];
        // _responseContent.UserState = UserState.UpdateItem;
        // _responseContent.KeyboardMarkup = new InlineKeyboardMarkup(new[]
        //     {
        //         new[]
        //         {
        //             InlineKeyboardButton.WithCallbackData("Nome"),
        //             InlineKeyboardButton.WithCallbackData("Marca"),
        //             InlineKeyboardButton.WithCallbackData("Preço")
        //         }
        //     }
        // );

        // return _responseContent;

        return _botResponse.GetResponse(request);
    }

    public ResponseContent CheckItemExistence(string nameAttribute)
    {
        // _responseContent.ResetResponseContent();

        var result = _itemRepository.GetItemInRepository(nameAttribute);

        if(result.Equals("item não encontrado.", StringComparison.CurrentCultureIgnoreCase))
        {
            // _responseContent.UserState = UserState.UpdateList;
            //_responseContent.AdditionalResponseContext = "Item no exist";

            ResponseContent response = _botResponse.GetResponse("Non-existent Item");
            response.Text += _itemRepository.GetList();
            return response;
        }

        if(result.Contains('\n'))
        {
            // _responseContent.Text = $"Encontrei os seguintes itens, qual deles você quer alterar?";
            // _responseContent.UserState = UserState.UpdateList;
            return _botResponse.GetResponse("More Than One Item");
        }

        return _botResponse.GetResponse("Update Item");
    }

    public ResponseContent AddItemInShoppingData(string userItems)
    {
        // _responseContent.ResetResponseContent();
        // _responseContent.Text = ResponseMessage["Item Added"];

        _itemRepository.AddItemInList(userItems);
        return _botResponse.GetResponse("Item Added");
    }

    public ResponseContent SendItemToUpdateList(string item)
    {
        // _responseContent.ResetResponseContent();
        // _responseContent.UserState = UserState.None;
        // _responseContent.Text = ResponseMessage["Update Item OK"];

        _itemRepository.UpdateList(item);
        return _botResponse.GetResponse("Update Item OK");
    }

    public ResponseContent SendItemToRemoveFromList(string item)
    {
        // _responseContent.ResetResponseContent();
        // _responseContent.Text = ResponseMessage["Deleted Item OK"];
        // _responseContent.UserState = UserState.None;

        _itemRepository.RemoveItemFromList(item);
        return _botResponse.GetResponse("Deleted Item OK");
    }
    
    public ResponseContent ShowList()
    {
        // _responseContent.ResetResponseContent();

        // _responseContent.Text = ResponseCallback["Show List"] + _itemRepository.GetList();
        // _responseContent.UserState = UserState.None;
        
        var response = _botResponse.GetResponse("Show List");
        response.Text += _itemRepository.GetList();
        return response;
    }

    public ResponseContent GetItemsToCreatelist(string items)
    {
        // _responseContent.ResetResponseContent();
        // _responseContent.Text = ResponseMessage["Created New List"];
        // _responseContent.UserState = UserState.None;

        //TODO: manipulate TimeStamp to formalize data
        _itemRepository.CreateNewList(items);
        return _botResponse.GetResponse("Created New List");
    }

    //TODO: implement verify function automatic of item
}