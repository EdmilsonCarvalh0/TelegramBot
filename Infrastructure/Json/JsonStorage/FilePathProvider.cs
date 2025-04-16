namespace TelegramBot.Infrastructure.Json.JsonStorage;

public class FilePathProvider
{
    private readonly string _filePathItems;
    private readonly string _filePathBotResponses;

    public FilePathProvider(string itemsFilePath, string botResponseFilePath)
    {
        _filePathItems = itemsFilePath;
        _filePathBotResponses = botResponseFilePath;
    }

    public string GetPath(FileType fileType) => fileType switch
    {
        FileType.Items => _filePathItems,
        FileType.BotResponses => _filePathBotResponses,
        _ => throw new ArgumentException("Tipo de arquivo não reconhecido.")
    };
}