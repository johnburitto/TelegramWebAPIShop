using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
    public class StartCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "/start" };

        public override async Task Execute(ITelegramBotClient client, Message message)
        {
            ReplyKeyboardMarkup keyboard = new(new[]
            {
                new KeyboardButton[] { "🎁 Всі товари" }
            })
            {
                ResizeKeyboard = true
            };

            await client.SendTextMessageAsync(chatId: message.Chat.Id, text: "Виберіть опцію:", replyMarkup: keyboard);
        }

        public override Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            throw new NotImplementedException();
        }
    }
}
