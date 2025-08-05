using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.UserInterface.Interfaces;

namespace TelegramBot.UserInterface;

public class KeyboardButtonFactory : IKeyboardButtonFactory
{
    public InlineKeyboardMarkup Create<T>(List<T> objects, int? numberOfButtons)
    {
        if(objects == null) throw new NullReferenceException();

        numberOfButtons = numberOfButtons ?? 2; // Define um valor padrão se não for especificado

        return objects.Select((o, i) => new { Index = i, Object = o })
            .GroupBy(x => x.Index / numberOfButtons)
            .Select(g => g
                .Select(x => InlineKeyboardButton.WithCallbackData(
                    x.Object.ToString(), 
                    x.Object.ToString()))
                .ToArray())
            .ToArray();
    }
}