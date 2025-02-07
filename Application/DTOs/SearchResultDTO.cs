using TelegramBot.Data;
using TelegramBot.Infrastructure;

namespace TelegramBot.Service;

public class SearchResultDTO
{
    public string Result { get; set; } = string.Empty;
    public SearchStatus Status { get; set; }
    
}