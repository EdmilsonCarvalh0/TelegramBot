using Newtonsoft.Json;
using TelegramBot.Infrastructure.Json.JsonStorage;

namespace TelegramBot.Infrastructure.Json;

public class JsonFileReader : IJsonFileReader
{
    private readonly FilePathProvider _filePathProvider;

    public JsonFileReader(FilePathProvider filePathProvider)
    {
        _filePathProvider = filePathProvider;
    }
    
    public T ReadFromFile<T>(FileType fileType)
    {
        var path = _filePathProvider.GetPath(fileType);
        var content = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(content)!;
    }

    public void WriteToFile<T>(FileType fileType, T content)
    {
        var path = _filePathProvider.GetPath(fileType);
        File.WriteAllText(path, JsonConvert.SerializeObject(content, Formatting.Indented));
    }
}