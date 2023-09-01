using Telegram.Bot.Types;
using Telegram.Bot;

namespace Bot.Base
{
    public interface ITelegramBotHandlers
    {
        Task MessageHandlerAsync(ITelegramBotClient client, Update update, CancellationToken token);
        Task ErrorHandlerAsync(ITelegramBotClient client, Exception exception, CancellationToken token);
    }
}
