using System;
using DigitalAgency.Api.Validate;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Services;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Bll.TelegramBot;
using DigitalAgency.Bll.TelegramBot.Services;
using DigitalAgency.Bll.TelegramBot.Services.Helpers;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Bll.TelegramBot.Services.Menus;
using DigitalAgency.Dal.Context;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace DigitalAgency.Api.Extensions;

public static class AddServicesExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Configure<BotConfiguration>(configuration.GetSection("BotConfiguration"))
            .AddTransient<IClientService, ClientService>()
            .AddTransient<IEnumService, EnumService>()
            .AddTransient<IProjectService, ProjectService>()
            .AddTransient<IOrderService, OrderService>()
            .AddTransient(typeof(DownloadFileExtension))
            .AddTransient<IButtons, Buttons>()
            .AddTransient<IExecutorMenuHelper, ExecutorMenuHelper>()
            .AddTransient<IClientMenuHelper, ClientMenuHelper>()
            .AddTransient<IRegistrationService, RegistrationService>()
            .AddTransient<IBotService, BotService>()
            .AddTransient<IExecutorMenu, ExecutorMenu>()
            .AddTransient<IClientMenu, ClientMenu>()
            .AddSingleton<ITelegramBotClient, TelegramBotClient>(provider =>
            {
                IOptions<BotConfiguration> options = provider.GetRequiredService<IOptions<BotConfiguration>>();
                if (!string.IsNullOrEmpty(options.Value?.BotToken))
                {
                    Console.WriteLine("Telegram configured");
                    return new TelegramBotClient(options.Value.BotToken);
                }

                Console.WriteLine("Telegram not configured");

                return null!;
            })
            .AddHostedService<MigrationsService>()
            .AddTransient<IValidator<ClientModel>, ClientModelValidator>()
            .AddTransient<IValidator<ProjectModel>, ProjectModelValidator>()
            .AddTransient<IValidator<ExecutorModel>, ExecutorModelValidator>()
            .AddTransient<IValidator<CardModel>, TaskModelValidator>();
    }
}