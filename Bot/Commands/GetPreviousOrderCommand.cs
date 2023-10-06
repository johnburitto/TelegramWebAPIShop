using Bot.Estensions;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Core.Entities;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
    public class GetPreviousOrderCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "order_prv" };

        public override Task Execute(ITelegramBotClient client, Message message)
        {
            throw new NotImplementedException();
        }

        public async override Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            var response = await RequestClient.Client.GetAsync($"api/Order/user/{callbackQuery.From?.Id}");
            var orders = JsonConvert.DeserializeObject<List<Order>>(await response.Content.ReadAsStringAsync());
            var mathes = Regex.Match(callbackQuery.Message?.Text ?? "", @"\[\d+\]");
            var id = mathes.Captures.First().Value.Replace("[", "").Replace("]", "");
            var order = orders?.GetPreviousOrder(int.Parse(id));

            if (order?.Id == int.Parse(id))
            {
                return;
            }

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

            await client.EditMessageTextAsync(callbackQuery!.Message!.Chat,
                                              callbackQuery!.Message!.MessageId,
                                              parseMode: ParseMode.Html,
                                              text: $"{order?.Status.ToEmoji()} <b>Замовлення [{order?.Id}]</b>\n\n" +
                                                    $"Товари: {order?.Products?.ToOrderString()}\n" +
                                                    $"Зроблено: {order?.CreatedAt.ToLongDateString()} {order?.CreatedAt.ToShortTimeString()}",
                                              replyMarkup: keyboard);
        }
    }
}
