using Newtonsoft.Json;
using TelegramBot.UserInterface;

namespace TelegramBot.Data;

public class BotResponse : BotResponseDataModel
{
    public BotResponseDataFormatter DataFormatter { get; } = new();
    private string ResponseJsonFilePath = "C:/Users/ANGELA SOUZA/OneDrive/Área de Trabalho/ED/Programação/C#/Projects/TelegramBot/Data/BotResponseModel/botResponseData.json";

    public BotResponse()
    {
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