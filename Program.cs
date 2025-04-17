using Domain.Item;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Application;
using TelegramBot.Application.DTOs;
using TelegramBot.Application.Handlers;
using TelegramBot.Application.Handlers.Interface;
using TelegramBot.Domain.Item.Input;
using TelegramBot.Infrastructure;
using TelegramBot.Infrastructure.Json;
using TelegramBot.Infrastructure.Json.JsonStorage;
using TelegramBot.Infrastructure.Telegram;
using TelegramBot.Service;
using TelegramBot.Service.ItemRepository;
using TelegramBot.Service.ShoppingAssistant;
using TelegramBot.Service.ShoppingAssistant.Utils;
using TelegramBot.UserInterface;

namespace TelegramBot
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    }
                )
                .ConfigureServices((services) =>
                    {
                        services.AddSingleton<ITelegramBotClient>(provider =>
                        {
                            var configuration = provider.GetRequiredService<IConfiguration>();
                            var botToken = configuration["BotSettings:Token"]
                                            ?? throw new InvalidOperationException("Token do bot não foi encontrado.");
                            
                            return new TelegramBotClient(botToken);
                        });
                        services.AddSingleton<ChatIdIdentifier>(provider =>
                        {
                            var configuration = provider.GetRequiredService<IConfiguration>();
                            var userId = configuration["BotSettings:UserId"]
                                        ?? throw new InvalidOperationException("UserId não encontrado.");
                            
                            return new ChatIdIdentifier(long.Parse(userId));
                        });
                        services.AddSingleton<SearchResultHandler>();
                        services.AddSingleton<FilePathProvider>(provider =>
                        {
                            var configuration = provider.GetRequiredService<IConfiguration>();
                            var botResponseFilePath = configuration["FilePaths:ResponsesRepository"]
                                        ?? throw new InvalidOperationException("Caminho de arquivo para repositório de informações de resposta não encontrado.");
                            var itemsFilePath = configuration["FilePaths:ItemsRepository"]
                                        ?? throw new InvalidOperationException("Caminho de arquivo para repositório de itens não encontrado.");
                            
                            return new FilePathProvider(itemsFilePath ,botResponseFilePath);
                        });
                        services.AddSingleton<IItemRepository, JsonItemRepository>();
                        services.AddSingleton<BotResponse>();
                        services.AddSingleton<ShoppingAssistantMode>();
                        services.AddSingleton<BotConnection>();
                        services.AddSingleton<UpdateHandlers>();
                        services.AddSingleton<UserStateManager>();
                        services.AddSingleton<IResponseManager, ResponseManager>();
                        services.AddSingleton<MessageSender>();
                        services.AddSingleton<ItemDataFormatter>();
                        services.AddSingleton<CallbackQuery>();
                        services.AddSingleton<Message>();
                        services.AddSingleton<HandlerContext>();
                        services.AddSingleton<IUpdateHandlerFactory, UpdateHandlerFactory>();
                        services.AddSingleton<IInputItemService, InputItemService>();
                        services.AddSingleton<IJsonFileReader, JsonFileReader>();
                        services.AddSingleton<StagingArea>();
                        services.AddScoped<IEditingArea, EditingArea>();
                        services.AddScoped<BotRequestContextFactory>();
                    }
                );

            var app = builder.Build();

            var bot = app.Services.GetRequiredService<BotConnection>();

            await bot.DisplayBotInfoAsync();
            bot.Start();

            await app.RunAsync();
        }
    }
}
