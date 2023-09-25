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
    public class GetCartCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "🛒 Моя корзина" };

        public override async Task Execute(ITelegramBotClient client, Message message)
        {
            var response = await RequestClient.Client.GetAsync($"api/Cart/{message.From?.Id}");

            if (await response.Content.ReadAsStringAsync() == string.Empty || await response.Content.ReadAsStringAsync() == "[]")
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Ваша корзина порожня 🫙");

                return;
            }

            var cartDictionary = JsonConvert.DeserializeObject<List<int>>(await response.Content.ReadAsStringAsync())?.ToCartDictionary();
            var product = JsonConvert.DeserializeObject<Product>(
                await (await RequestClient.Client.GetAsync($"api/Product/{cartDictionary?.First().Key}")).Content.ReadAsStringAsync());
            var photo = await RequestClient.Client.GetAsync(product?.Thumbnails?.First()?.URI ?? "");
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Попередній", "cart_prv"),
                    InlineKeyboardButton.WithCallbackData("Наступний ➡️", "cart_nxt")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Видалити одну одиницю з корзини", "remove_one_from_cart"),
                    InlineKeyboardButton.WithCallbackData("Видалити все з корзини", "remove_all_from_cart")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData($"Загальна ціна: {await cartDictionary!.TotalPriceAsync() ?? 0f}", "total_price")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Офоромити замовлення", "start_ordering")
                }
            });

            await client.SendPhotoAsync(message.Chat.Id, parseMode: ParseMode.Html,
                photo: InputFile.FromStream(photo.Content.ReadAsStream()),
                caption: $"<b>{product?.Name} [{product?.Id}]</b> x{cartDictionary?.Where(el => el.Key == product?.Id).First().Value}\n\n" +
                         $"{product?.Description}\n\n",
                replyMarkup: keyboard);
        }

        public override Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            throw new NotImplementedException();
        }
    }
}
