using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands
{
    public class ReplyCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "/reply" };

        public override async Task Execute(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"Ви написали:\n{message.Text?.Replace("/reply ", "")}");
        }
    }
}
