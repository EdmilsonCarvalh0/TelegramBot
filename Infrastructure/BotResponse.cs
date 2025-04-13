using Newtonsoft.Json;
using TelegramBot.Application;
using TelegramBot.Application.Bot;
using TelegramBot.Infrastructure.JsonStorage;

namespace TelegramBot.Infrastructure;

public class BotResponse
{
    public BotResponseCollection DataFormatter { get; } = new();
    private string ResponseJsonFilePath;

    public BotResponse(string filePath)
    {
        ResponseJsonFilePath = filePath;
        DataFormatter = LoadData();
    }

    private BotResponseCollection LoadData()
    {
        return JsonConvert.DeserializeObject<BotResponseCollection>(File.ReadAllText(ResponseJsonFilePath))!;
    }

    public ResponseContent GetResponse(string request)
    {
        return new ResponseContent {
            Text = DataFormatter.Responses[request].Text,
            KeyboardMarkup = DataFormatter.Responses[request].KeyboardMarkup,
            UserState = DataFormatter.Responses[request].UserState
        };
    }
}