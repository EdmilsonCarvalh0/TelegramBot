using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;
using TelegramBot.UserInterface;

public interface IItemController
{
    ResponseContent GetResponseCallback(string subject);
    ResponseContent GetResponseMessage(string subject);
    ResponseContent GetInitialMessage();
    ResponseContent StartService(string request);
    ResponseContent GetOptionsOfListUpdate(string request);
    ResponseContent GetAttributeOptions(string request);
    ResponseContent CheckItemExistence(string nameAttribute);
    ResponseContent AddItemInShoppingData(string item);
    ResponseContent SendItemToUpdateList(string item);
    ResponseContent SendItemToRemoveFromList(string item);
    ResponseContent ShowList();
    ResponseContent GetItemsToCreatelist(string item);
}