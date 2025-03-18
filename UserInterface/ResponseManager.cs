using TelegramBot.Data;
using TelegramBot.Application;
using TelegramBot.Domain;
using TelegramBot.Service;
using TelegramBot.Infrastructure;

namespace TelegramBot.UserInterface;

//TODO: Create Class about List/Data for encapsulate functionalities
//      criar classe sobre list/dados para encapsular funcionalidades
public class ResponseManager : IResponseManager
{
    private readonly IItemRepository _itemRepository; //REMOVE
    private readonly MessageSender _messageSender;
    private BotResponse _botResponse;

    public ResponseManager(IItemRepository itemRespository, MessageSender messageSender, BotResponse botResponse)
    {
        _itemRepository = itemRespository;
        _messageSender = messageSender;
        _botResponse = botResponse;
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

        _itemRepository.AddItemInEditingArea(nameAttribute);

        return _botResponse.GetResponse("Update Item");
    }

    public ResponseContentDTO AddItemInShoppingData(string userItems)
    {
        _itemRepository.AddItemInList(userItems);
        return _botResponse.GetResponse("Item Added");
    }

    public ResponseContentDTO SendAttributeToUpdateItem(string attribute)
    {
        _itemRepository.UpdateItemInList(attribute);
        return _botResponse.GetResponse("Update Item OK");
    }

    public void SendAttributeToEditingArea(string attributeToBeChagend)
    {
        _itemRepository.AddAttributeToBeChangedInEditingArea(attributeToBeChagend);
    }

    public ResponseContentDTO SendItemToRemoveFromList(string item)
    {
        var result = _itemRepository.RemoveItemFromList(item);

        if (result.Status == SearchStatus.NotFound)
        {
            var response = _botResponse.GetResponse("Non-existent Item");
            response.Text += _itemRepository.GetList();
            return response;
        }

        if (result.Status == SearchStatus.MoreThanOne)
        {
            var response = _botResponse.GetResponse("More Than One Item");
            response.Text += result.Result;
            return response;
        }

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