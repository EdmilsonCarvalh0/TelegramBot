using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.UserInterface.Interfaces;

public interface IKeyboardButtonFactory
{
    InlineKeyboardMarkup Create<T>(List<T> objects, int? numberOfButtons);
}