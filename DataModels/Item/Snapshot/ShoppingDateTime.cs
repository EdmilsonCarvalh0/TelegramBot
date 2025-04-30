using System.Globalization;

namespace TelegramBot.DataModels.Item.Snapshot;

public class ShoppingDateTime
{
    public string Month { get;  set; }
    public int Day { get; set; }
    public string Hour { get; set; }

    public static ShoppingDateTime Default { get; set; } = new("Desconhecido", 0, "--:--");

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