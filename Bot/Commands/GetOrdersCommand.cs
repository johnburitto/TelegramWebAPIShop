using Bot.Estensions;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Core.Entities;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
    public class GetOrdersCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "📃 Мої замовлення" };

        public override async Task Execute(ITelegramBotClient client, Message message)
        {
            var response = await RequestClient.Client.GetAsync($"api/Order/user/{message.From?.Id}");
            var orders = JsonConvert.DeserializeObject<List<Order>>(await response.Content.ReadAsStringAsync());
            var order = orders?.First();
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Попереднє", "order_prv"),
                    InlineKeyboardButton.WithCallbackData("Наступне ➡️", "order_nxt")
                },
                new[]
                {
                    order?.Status == OrderStatus.Created || order?.Status == OrderStatus.Approved ? 
                        InlineKeyboardButton.WithCallbackData("Оплатити", "pay") : InlineKeyboardButton.WithCallbackData("✅ Оплачено")
                }
            });

            await client.SendTextMessageAsync(message.Chat.Id, 
                                              parseMode: ParseMode.Html,
                                              text: $"{order?.Status.ToEmoji()} <b>Замовлення [{order?.Id}]</b>\n\n" +
                                                    $"Товари: {order?.Products?.ToOrderString()}\n" +
                                                    $"Зроблено: {order?.CreatedAt.ToLongDateString()} {order?.CreatedAt.ToShortTimeString()}",
                                              replyMarkup: keyboard);
        }

        public override Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            throw new NotImplementedException();
        }
    }
}
