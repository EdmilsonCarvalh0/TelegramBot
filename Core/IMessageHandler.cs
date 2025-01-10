using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;

public interface IMessageHandler
{
    InlineKeyboardMarkup StartService();
    InlineKeyboardMarkup GetOptionsOfListUpdate();
    InlineKeyboardMarkup GetAttributeOptions();
    string AddItemInShoppingData(string item);
    string SendItemToUpdateList(string item);
    string SendItemToRemoveFromList(string item);
    string ShowList();
    string GetItemsToCreatelist(string item);
}