using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands
{
    public abstract class Command
    {
        protected abstract List<string> Names { get; set; }

        public abstract Task Execute(ITelegramBotClient client, Message message);

        public async Task TryExecute(ITelegramBotClient client, Message message)
        {
            if (IsContains(message.Text ?? string.Empty))
            {
                await Execute(client, message);
            }
        }

        public bool IsContains(string message)
        {
            foreach(var name in  Names)
            {
                if (message.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
