using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Test_Work.Abstractions;
using Test_Work.Data;
using Test_Work.Options;
using Test_Work.Services;

namespace Test_Work;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var mySqlVersion = new MySqlServerVersion(new Version(configuration["DatabaseSettings:MySqlVersion"]));

        services.AddDbContext<ReminderDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("MySqlConnection"),
                mySqlVersion
            )
        );

        services.AddSingleton<IStateService, StateService>();
        services.AddScoped<IBotService, BotService>();
        services.AddScoped<ICalculatorService, CalculatorService>();
        services.AddScoped<IReminderService, ReminderService>();

        services.Configure<TelegramBotSettings>(configuration.GetSection("TelegramBot"));

        services.AddHostedService<TelegramBotHostedService>();
        services.AddHostedService<ReminderHostedService>();

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TelegramBotSettings>>().Value;

            return new TelegramBotClient(options.Token);
        });
    }
}
