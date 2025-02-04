namespace TelegramBot.Domain;

public enum UserState
{
    None,
    ServicePaused,
    Running,
    GetList,
    UpdateList,
    AddItem,
    UpdateItem,
    DeleteItem,
    CreateList
}