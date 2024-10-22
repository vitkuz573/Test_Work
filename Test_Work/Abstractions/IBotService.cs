using Telegram.Bot.Types;

namespace Test_Work.Abstractions;

public interface IBotService
{
    Task HandleUpdateAsync(Update update);
}
