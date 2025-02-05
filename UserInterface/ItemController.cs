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

    public ItemController()
    {
        _itemRepository = new JsonItemRepository();
        _botResponse = new BotResponse();
    }

    public ResponseContent GetResponseCallback(string subject)
    {
        return _botResponse.GetResponse(subject);
    }

    public ResponseContent GetResponseMessage(string subject)
    {
        return _botResponse.GetResponse(subject);
    }

    public ResponseContent GetInitialMessage(string subject)
    {   
        return _botResponse.GetResponse(subject);
    }

    public ResponseContent StartService(string request)
    {
        return _botResponse.GetResponse(request);
    }

    public ResponseContent GetOptionsOfListUpdate(string request)
    {
        return _botResponse.GetResponse(request);
    }

    public ResponseContent GetAttributeOptions(string request)
    {
        return _botResponse.GetResponse(request);
    }

    public ResponseContent CheckItemExistence(string nameAttribute)
    {
        var result = _itemRepository.GetItemInRepository(nameAttribute);

        if(result.Equals("item não encontrado.", StringComparison.CurrentCultureIgnoreCase))
        {
            var response = _botResponse.GetResponse("Non-existent Item");
            response.Text += _itemRepository.GetList();
            return response;
        }

        if(result.Contains('\n'))
        {
            var response = _botResponse.GetResponse("More Than One Item");
            response.Text += result;
            return response;
        }

        return _botResponse.GetResponse("Update Item");
    }

    public ResponseContent AddItemInShoppingData(string userItems)
    {
        _itemRepository.AddItemInList(userItems);
        return _botResponse.GetResponse("Item Added");
    }

    public ResponseContent SendItemToUpdateList(string item)
    {
        _itemRepository.UpdateList(item);
        return _botResponse.GetResponse("Update Item OK");
    }

    public ResponseContent SendItemToRemoveFromList(string item)
    {
        _itemRepository.RemoveItemFromList(item);
        return _botResponse.GetResponse("Deleted Item OK");
    }
    
    public ResponseContent ShowList()
    {
        var response = _botResponse.GetResponse("Show List");
        response.Text += _itemRepository.GetList();
        return response;
    }

    public ResponseContent GetItemsToCreatelist(string items)
    {
        //TODO: manipulate TimeStamp to formalize data
        _itemRepository.CreateNewList(items);
        return _botResponse.GetResponse("Created New List");
    }

    //TODO: implement verify function automatic of item
}