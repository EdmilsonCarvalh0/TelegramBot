using Telegram.Bot.Types;
using TelegramBot.Application.Handlers;
using TelegramBot.Infrastructure;

namespace TelegramBot.Application;

public class UserStateManager
{
    private readonly Dictionary<long, StateContext> _userStates;
    // private readonly IFLowManager _flowManager = new FlowManager();

    public UserStateManager(ChatIdIdentifier userId)
    {
        _userStates = new()
        {
            { userId.BotId, new StateContext() }
        };
        // _userStates = new()
        // {
        //     { default,
        //         new StateContext()
        //         {
        //             State = InteractionState.Running,
        //             CurrentStateOfFlow = InteractionState.Running
        //         }
        //     }
        // };
    }

    public void CheckDataButton(long userId, CallbackQuery callbackQuery)
    {
        // AVALIAR CRIAÇÃO DE UMA CLASSE QUE SEJA RESPONSÁVEL POR DEFINIR INICIALMENTE CADA STATE
        
        if (callbackQuery != null && callbackQuery!.Data == "Ver listas")
        {
            _userStates[userId].State = InteractionState.ListViewEnabled;
            _userStates[userId].StartStateFlow();
        }

        if (callbackQuery != null && callbackQuery!.Data == "Modo Ficando Mais Pobre")
        {
            _userStates[userId].State = InteractionState.ShoppingModeEnabled;
            _userStates[userId].StartStateFlow();
        }
    }

    public InteractionState GetInteractionState(long userId)
    {
        return _userStates[userId].State;
    }

    public Enum GetCurrentStateOfFlow(long userId)
    {
        return _userStates[userId].CurrentStateOfFlow;
    }

    public void CheckIfContainsUserId(long userId)
    {
        if (!_userStates.ContainsKey(userId))
        {
            SetUserId(userId);
        }
    }

    private void SetUserId(long userId)
    {
        if (_userStates.TryGetValue(userId, out var value))
        {
            _userStates.Remove(userId);
            _userStates.Add(userId, value);
        }
    }

    public void SetState(long userId, InteractionState state)
    {
        _userStates[userId].State = state;
    }

    public void SetStateFlow(long userId, Enum state)
    {
        _userStates[userId].CurrentStateOfFlow = state;
    }

    public Enum GetStateFlow(long userId)
    {
        return _userStates[userId].CurrentStateOfFlow;

        // return _userStates.TryGetValue(userId, out var state) ?
        //     state :
        //     new StateContext { State = InteractionState.InitialMessage };
    }
    

    //public void SetAdditionalInfo(long userId, string additionalInfo)
    //{
    //    _userStates[userId].AdditionalInfo = additionalInfo;
    //}

    //public void ResetAdditionalInfo(long userId)
    //{
    //    _userStates[userId].AdditionalInfo = "";
    //}


}