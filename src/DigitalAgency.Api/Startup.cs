using System;
using System.Collections.Generic;
using System.Reflection;
using DigitalAgency.Api.Common;
using DigitalAgency.Api.Validate;
using DigitalAgency.Bll;
using DigitalAgency.Bll.AutoMapper;
using DigitalAgency.Bll.Models;
using DigitalAgency.Dal.Context;
using DigitalAgency.Dal.Storages;
using DigitalAgency.Dal.Storages.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Telegram.Bot;

namespace DigitalAgency.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ServicingContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")),ServiceLifetime.Transient);
            services.AddAutoMapper(typeof(AutoMapperProfile).GetTypeInfo().Assembly);

            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddApplicationInsightsTelemetry();
            //services.AddTransient<IBotService, BotService>();
            services.AddTransient<IClientStorage, ClientStorage>();
            services.AddTransient<IExecutorStorage, ExecutorStorage>();
            services.AddTransient<IProjectStorage, ProjectStorage>();
            services.AddTransient<IOrderStorage, OrderStorage>();
            services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"))
                .AddSingleton<ITelegramBotClient,TelegramBotClient>(provider => { 
                    var options = provider.GetRequiredService<IOptions<BotConfiguration>>();
                    if (!string.IsNullOrEmpty(options.Value?.BotToken))
                    {
                        Console.WriteLine("Telegram configured");
                        return new TelegramBotClient(options.Value.BotToken);
                    }
                    
                    Console.WriteLine("Telegram not configured");
                    
                    return null!; 
                });
            
            services.AddHostedService<MigrationsService>();
            services.AddSwaggerGen(c =>
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

            services.AddTransient<IValidator<ClientModel>, ClientModelValidator>();
            services.AddTransient<IValidator<ProjectModel>, ProjectModelValidator>();
            services.AddTransient<IValidator<ExecutorModel>, ExecutorModelValidator>();
            services.AddTransient<IValidator<TaskModel>, TaskModelValidator>();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCustomExceptionHandler();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital Agency API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
