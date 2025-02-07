using TelegramBot.Data;
using TelegramBot.Application;
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

    public ResponseContentDTO GetResponseCallback(string subject)
    {
        return _botResponse.GetResponse(subject);
    }

    public ResponseContentDTO GetResponseMessage(string subject)
    {
        return _botResponse.GetResponse(subject);
    }

    public ResponseContentDTO GetInitialMessage(string subject)
    {   
        return _botResponse.GetResponse(subject);
    }

    public ResponseContentDTO StartService(string request)
    {
        return _botResponse.GetResponse(request);
    }

    public ResponseContentDTO GetOptionsOfListUpdate(string request)
    {
        return _botResponse.GetResponse(request);
    }

    public ResponseContentDTO GetAttributeOptions(string request)
    {
        return _botResponse.GetResponse(request);
    }

    public ResponseContentDTO CheckItemExistence(string nameAttribute)
    {
        var result = _itemRepository.GetItemInRepository(nameAttribute);

        if(result.Status == SearchStatus.NotFound)
        {
            var response = _botResponse.GetResponse("Non-existent Item");
            response.Text += _itemRepository.GetList();
            return response;
        }

        if(result.Status == SearchStatus.MoreThanOne)
        {
            var response = _botResponse.GetResponse("More Than One Item");
            response.Text += result.Result;
            return response;
        }

        return _botResponse.GetResponse("Update Item");
    }

    public ResponseContentDTO AddItemInShoppingData(string userItems)
    {
        _itemRepository.AddItemInList(userItems);
        return _botResponse.GetResponse("Item Added");
    }

    public ResponseContentDTO SendItemToUpdateList(string item)
    {
        _itemRepository.UpdateList(item);
        return _botResponse.GetResponse("Update Item OK");
    }

    public ResponseContentDTO SendItemToRemoveFromList(string item)
    {
        _itemRepository.RemoveItemFromList(item);
        return _botResponse.GetResponse("Deleted Item OK");
    }
    
    public ResponseContentDTO ShowList()
    {
        var response = _botResponse.GetResponse("Show List");
        response.Text += _itemRepository.GetList();
        return response;
    }

    public ResponseContentDTO GetItemsToCreatelist(string items)
    {
        //TODO: manipulate TimeStamp to formalize data
        _itemRepository.CreateNewList(items);
        return _botResponse.GetResponse("Created New List");
    }

    //TODO: implement verify function automatic of item
}