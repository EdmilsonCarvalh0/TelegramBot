using TelegramBot.Infrastructure.Json.JsonStorage;

namespace TelegramBot.Infrastructure.Json;

public interface IJsonFileReader
{
    T ReadFromFile<T>(FileType fileType);
    void WriteToFile<T>(FileType fileType, T content);
}