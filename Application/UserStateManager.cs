using TelegramBot.Domain;

namespace TelegramBot.Application;

public class UserStateManager
{
    private readonly Dictionary<long, UserStateData> _userStates = new();

    public UserStateManager()
    {
        _userStates.Add(default, new UserStateData {
            State = UserState.None,
            LastUpdated = DateTime.UtcNow
        });
    }

    public UserStateData GetUserStateData(long userId)
    {
        return _userStates.TryGetValue(userId, out var state) ? state : new UserStateData {
                                                                        State = UserState.None,
                                                                        LastUpdated = DateTime.UtcNow
        };
    }

    public void SetState(long userId, UserState state)
    {
        _userStates[userId].State = state;
    }

    public void SetaAdditionalInfo(long userId, string additionalInfo)
    {
        _userStates[userId].AdditionalInfo = additionalInfo;
    }

    public void ResetState(long userId)
    {
        _userStates.Remove(userId);
    }
}