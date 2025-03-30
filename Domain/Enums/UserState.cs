namespace TelegramBot.Domain;

public enum UserState
{
    None,
    Running,
    GetList,
    UpdateList,
    AddItem,
    UpdateItem,
    DeleteItem,
    CreateList,
    ConfirmShoppingMode,
    PreparingShoppingMode,
    OnShoppingMode
}