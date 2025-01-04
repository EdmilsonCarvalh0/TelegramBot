using TelegramBot.Data;

public interface IShoppingData
{
    void LoadData();
    string GetList();
    void UpdateList(string items);
    void AddItemInList(string items);
    void RemoveItemFromList(string item);
    void CreateNewList(string items);
}