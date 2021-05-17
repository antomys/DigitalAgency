using System;
using System.Collections.Generic;
using DigitalAgency.Api.Validate;
using DigitalAgency.Bll;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Services;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Context;
using DigitalAgency.Dal.Storages;
using DigitalAgency.Dal.Storages.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Telegram.Bot;

namespace DigitalAgency.Api.Extensions
{
    public static class AddServicesExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            //.AddTransient<IBotService, BotService>();
            return services
                .AddTransient<IClientService, ClientService>()
                .AddTransient<IProjectService, ProjectService>()
                .AddTransient<IOrderService, OrderService>()
                .AddTransient<IEnumService, EnumService>()
            .Configure<BotConfiguration>(configuration.GetSection("BotConfiguration"))
                .AddSingleton<ITelegramBotClient,TelegramBotClient>(provider => { 
                    var options = provider.GetRequiredService<IOptions<BotConfiguration>>();
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
            .AddTransient<IValidator<TaskModel>, TaskModelValidator>()
            
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    },
                });
            });
        }
 

        public static IServiceCollection AddStorages(this IServiceCollection services)
        {
            return services
                .AddTransient<IClientStorage, ClientStorage>()
                .AddTransient<IExecutorStorage, ExecutorStorage>()
                .AddTransient<IProjectStorage, ProjectStorage>()
                .AddTransient<IOrderStorage, OrderStorage>();
        }
    }
}