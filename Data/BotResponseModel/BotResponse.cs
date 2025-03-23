using Newtonsoft.Json;
using TelegramBot.Application;

namespace TelegramBot.Data;

public class BotResponse : BotResponseDataModel
{
    public BotResponseDataFormatter DataFormatter { get; } = new();
    private string ResponseJsonFilePath;

    public BotResponse(string filePath)
    {
        ResponseJsonFilePath = filePath;
        DataFormatter = LoadData();
    }

    private BotResponseDataFormatter LoadData()
    {
        return JsonConvert.DeserializeObject<BotResponseDataFormatter>(File.ReadAllText(ResponseJsonFilePath))!;
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