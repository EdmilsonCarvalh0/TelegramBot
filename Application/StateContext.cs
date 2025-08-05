using TelegramBot.Application.Handlers;
using TelegramBot.Application.Handlers.InitialMessage;
using TelegramBot.Application.Handlers.ListViewAccess;
using TelegramBot.Application.Handlers.ShoppingMode;

namespace TelegramBot.Application;

public class StateContext
{
    public InteractionState State { get; set; }
    public Enum CurrentStateOfFlow { get; set; }

    public StateContext()
    {
        State = InteractionState.InitialMessage;
        CurrentStateOfFlow = InitialMessageState.InformHowToAccessMenu;
    }

    public void StartStateFlow()
    {
        switch (State)
        {
            case InteractionState.InitialMessage:
                CurrentStateOfFlow = InitialMessageState.InformHowToAccessMenu;
                break;
            case InteractionState.ListViewEnabled:
                CurrentStateOfFlow = ListViewAccessState.PresentOptionsFromExistingLists;
                break;
            case InteractionState.ShoppingModeEnabled:
                CurrentStateOfFlow = ShoppingModeState.ConfirmShoppingMode;
                break;
        }
    }
}