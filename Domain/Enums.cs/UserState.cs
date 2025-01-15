namespace TelegramBot.Domain;

public enum UserState
{
    None,
    ServicePaused,
    ServiceStarted,
    GetList,
    UpdateList,
    AddItem,
    UpdateItem,
    DeleteItem,
    CreatingNewList
}