using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;
using TelegramBot.UserInterface;

public interface IItemController
{
    ResponseContent GetInitialMessage();
    ResponseContent StartService();
    ResponseContent GetOptionsOfListUpdate();
    ResponseContent GetAttributeOptions();
    ResponseContent CheckItemExistence(string nameAttribute);
    ResponseContent AddItemInShoppingData(string item);
    ResponseContent SendItemToUpdateList(string item);
    string SendItemToRemoveFromList(string item);
    string ShowList();
    string GetItemsToCreatelist(string item);
}