using TelegramBot.Application.Bot;
using TelegramBot.Infrastructure.Json;
using TelegramBot.Infrastructure.JsonStorage;
using FileType = TelegramBot.Infrastructure.Json.JsonStorage.FileType;

namespace TelegramBot.Infrastructure;

public class BotResponse
{
    private BotResponseCollection ResponseCollection { get; }
    private readonly IJsonFileReader _fileReader;


    public BotResponse(IJsonFileReader fileReader)
    {
        _fileReader = fileReader;
        ResponseCollection = LoadData();
    }

    private BotResponseCollection LoadData()
    {
        return _fileReader.ReadFromFile<BotResponseCollection>(FileType.BotResponses);
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