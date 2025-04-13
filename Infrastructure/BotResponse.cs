using Newtonsoft.Json;
using TelegramBot.Application;
using TelegramBot.Application.Bot;
using TelegramBot.Infrastructure.JsonStorage;

namespace TelegramBot.Infrastructure;

public class BotResponse
{
    private BotResponseCollection ResponseCollection { get; }
    private readonly string _responseJsonFilePath;

    public BotResponse(string filePath)
    {
        _responseJsonFilePath = filePath;
        ResponseCollection = LoadData();
    }

    private BotResponseCollection LoadData()
    {
        return JsonConvert.DeserializeObject<BotResponseCollection>(File.ReadAllText(_responseJsonFilePath))!;
    }

    public ResponseContent GetResponse(string request)
    {
        return new ResponseContent {
            Text = ResponseCollection.Responses[request].Text,
            KeyboardMarkup = ResponseCollection.Responses[request].KeyboardMarkup,
            UserState = ResponseCollection.Responses[request].UserState
        };
    }
}