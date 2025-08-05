using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Application.DTOs;

public class ResponseInfoToSendToTheUser
{
    public Enum? Subject { get; set; }
    public string? SubjectContextData { get; set; }
    public InlineKeyboardMarkup? KeyboardMarkup { get; set; }
}