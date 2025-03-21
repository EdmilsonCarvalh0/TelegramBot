using TelegramBot.Domain;

namespace TelegramBot.Service;

public class SearchResult
{
    public string Result { get; set; } = string.Empty;
    public SearchStatus Status { get; set; }

}