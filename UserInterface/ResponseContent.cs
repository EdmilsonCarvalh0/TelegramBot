
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Domain;
using TelegramBot.Infrastructure;

namespace TelegramBot.UserInterface
{
    public class ResponseContent
    {
        //TODO: avaliar o uso de GET e SET
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty("keyboardMarkup")]
        [Newtonsoft.Json.JsonConverter(typeof(InlineKeyboardMarkupConverter))]
        public InlineKeyboardMarkup? KeyboardMarkup { get; set; }

        [JsonProperty("userState")]
        public UserState UserState { get; set; }

        public ResponseContent()
        {
        }

    }
}
