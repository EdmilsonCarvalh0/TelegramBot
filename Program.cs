using Application.Handlers;
using Application.Handlers.Interface;
using Domain.Item;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Application;
using TelegramBot.Application.Handlers;
using TelegramBot.Data;
using TelegramBot.Infrastructure;
using TelegramBot.Service;
using TelegramBot.UserInterface;

namespace TelegramBot
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    }
                )
                .ConfigureServices((context, services) =>
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
                        services.AddSingleton<IItemRepository, JsonItemRepository>(provider =>
                        {
                            var configuration = provider.GetRequiredService<IConfiguration>();
                            var searchResultHandler = provider.GetRequiredService<SearchResultHandler>();
                            var serviceProvider = provider.GetRequiredService<IServiceProvider>();

                            var filePath = configuration["FilePaths:ItemsRepository"] 
                                        ?? throw new InvalidOperationException("Caminho de arquivo para repositório de itens não encontrado.");
                                        
                            return new JsonItemRepository(searchResultHandler, serviceProvider, filePath);
                        });
                        services.AddSingleton<BotResponse>(provider =>
                        {
                            var configuration = provider.GetRequiredService<IConfiguration>();
                            var filePath = configuration["FilePaths:ResponsesRepository"]
                                        ?? throw new InvalidOperationException("Caminho de arquivo para repositório de informações de resposta não encontrado.");
                            
                            return new BotResponse(filePath);
                        });
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
