using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;

public interface IBotClient
{
    InlineKeyboardMarkup StartService();
    InlineKeyboardMarkup GetOptionsOfListUpdate();
    string AddItemInShoppingData(string item);
    string SendItemToUpdateList(string item);
    string SendItemToRemoveFromList(string item);
    string ShowList();
    string GetItemsToCreatelist(string item);
}