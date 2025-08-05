using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Handlers;
using TelegramBot.Infrastructure;
using TelegramBot.Infrastructure.Json.JsonConverters;

namespace TelegramBot.Application.Bot
{
    public class ResponseContent
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty("keyboardMarkup")]
        [JsonConverter(typeof(InlineKeyboardMarkupConverter))]
        public InlineKeyboardMarkup? KeyboardMarkup { get; set; }

        [JsonProperty("userState")]
        public InteractionState UserState { get; set; }
    }
}
