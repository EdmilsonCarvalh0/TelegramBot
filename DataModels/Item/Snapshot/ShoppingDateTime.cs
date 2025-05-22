using System.Globalization;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace TelegramBot.DataModels.Item.Snapshot;

public class ShoppingDateTime
{
    [JsonProperty]
    public string Month { get; private set; }
    
    [JsonProperty]
    public int Day { get; private set; }
    
    [JsonProperty]
    public string Hour { get; private set; }

    [JsonConstructor]
    public ShoppingDateTime(string month, int day, string hour)
    {
        Month = month;
        Day = day;
        Hour = hour;
    }

    public void RegisterDateInfo()
    {
        var now = DateTime.Now;
        Month = now.ToString("MMMM", new CultureInfo("pt-BR"));
        Day = now.Day;
        Hour = now.ToString("HH:mm");
    }

    public static ShoppingDateTime GetDefault()
    {
        return new ShoppingDateTime("Desconhecido", 0, "--:--");
    }

    public override string ToString()
    {
        return $"{Month}, {Day}, {Hour}";
    }

    public override bool Equals(object? o)
    {
        if (o is null) return false;
        if (o is not ShoppingDateTime other) return false;
        
        return Month == other.Month &&
               Day == other.Day &&
               Hour == other.Hour;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Month, Day, Hour);
    }
}