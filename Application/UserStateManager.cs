using TelegramBot.Domain;

namespace TelegramBot.Application;

public class UserStateManager
{
    private static readonly Dictionary<long, UserStateData> _userStates = new(); //Verificar Dependence Injection

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
    
    public bool ContainsUserId(long userId)
    {
        return _userStates.ContainsKey(userId);
    }

    public void SetUserId(long userId)
    {
        UserStateData value = new ();
        
        if (_userStates.TryGetValue(0, out value!))
        {
            _userStates.Remove(0);
            _userStates.Add(userId, value);
        }
    }

    public static void SetState(long userId, UserState state)
    {
        _userStates[userId].State = state;
    }

    public void ResetState(long userId)
    {
        _userStates.Remove(userId);
    }

    public void SetAdditionalInfo(long userId, string additionalInfo)
    {
        _userStates[userId].AdditionalInfo = additionalInfo;
    }

    public void ResetAdditionalInfo(long userId)
    {
        _userStates[userId].AdditionalInfo = "";
    }
}