using TelegramBot.Domain;

namespace TelegramBot.Application;

public class UserStateData
{
    public UserState State { get; set; }
    public DateTime LastUpdated { get; set; }
    public string AdditionalInfo { get; set; }

    public UserStateData()
    {
        AdditionalInfo = "";
    }
}