using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Domain;

namespace TelegramBot.UserInterface
{
    public class ResponseContent
    {
        public string Text { get; set; } = string.Empty;
        public InlineKeyboardMarkup? KeyboardMarkup { get; set; }
        public UserState UserState { get; set; }
        public string AdditionalResponseContext { get; set; } = string.Empty;

        public void ResetResponseContent()
        {
            Text = string.Empty;
            KeyboardMarkup = null;
            UserState = UserState.None;
            AdditionalResponseContext = string.Empty;
        }
    }
}
