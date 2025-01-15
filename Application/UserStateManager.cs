/*
    Gerenciar estados do usuário durante o fluxo
*/

using TelegramBot.Domain;

namespace TelegramBot.Application;

public class UserStateManager
{
    private readonly Dictionary<long, UserState> _userStates = new();

    public UserState GetState(long userId)
    {
        return _userStates.TryGetValue(userId, out var state) ? state : UserState.None;
    }

    public void SetState(long userId, UserState state)
    {
        _userStates[userId] = state;
    }

    public void ResetState(long userId)
    {
        _userStates.Remove(userId);
    }
}