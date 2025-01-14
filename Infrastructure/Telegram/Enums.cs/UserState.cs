namespace TelegramBot.Infrastructure;

public enum UserState
{
    ServicePaused,
    ServiceStarted,
    WaitingForName,
    EditingItem
}