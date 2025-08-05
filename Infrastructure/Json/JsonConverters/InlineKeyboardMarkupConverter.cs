using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Infrastructure.Json.JsonConverters;

public class InlineKeyboardMarkupConverter : JsonConverter<InlineKeyboardMarkup>
{
    public override InlineKeyboardMarkup? ReadJson(JsonReader reader,
                                                   Type objectType,
                                                   InlineKeyboardMarkup? existingValue,
                                                   bool hasExistingValue,
                                                   JsonSerializer serializer)
    {
        var buttons = serializer.Deserialize<InlineKeyboardButton[][]>(reader);
        return buttons != null ? new InlineKeyboardMarkup(buttons) : null;
    }

    public override void WriteJson(JsonWriter writer, InlineKeyboardMarkup? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value?.InlineKeyboard);
    }
}