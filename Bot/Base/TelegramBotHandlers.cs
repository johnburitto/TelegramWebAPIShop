using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Base
{
    public class TelegramBotHandlers : ITelegramBotHandlers
    {
        public async Task MessageHandlerAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    await MessageHandlerAsync(client, update.Message ?? throw new Exception("Message can't be null"));
                }break;
            }
        }

        public Task ErrorHandlerAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private async Task MessageHandlerAsync(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"Ви написали:\n{message.Text}");
        }
    }
}
