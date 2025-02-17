using TelegramBot.Application;

public interface IItemController
{
    ResponseContentDTO GetResponseCallback(string subject);
    ResponseContentDTO GetResponseMessage(string subject);
    ResponseContentDTO GetInitialMessage(string subject);
    ResponseContentDTO StartService(string request);
    ResponseContentDTO GetOptionsOfListUpdate(string request);
    ResponseContentDTO GetAttributeOptions(string request);
    ResponseContentDTO CheckItemExistence(string nameAttribute);
    ResponseContentDTO AddItemInShoppingData(string item);
    ResponseContentDTO SendAttributeToUpdateItem(string item);
    void SendAttributeToEditingArea(string attributeToBeChagend);
    ResponseContentDTO SendItemToRemoveFromList(string item);
    ResponseContentDTO ShowList();
    ResponseContentDTO GetItemsToCreatelist(string item);
}