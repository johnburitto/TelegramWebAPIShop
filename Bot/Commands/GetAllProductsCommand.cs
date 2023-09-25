using Bot.HttpInfrastructure;
using Core.Entities;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
    public class GetAllProductsCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "🎁 Всі товари" };

        public override async Task Execute(ITelegramBotClient client, Message message)
        {
            var response = await RequestClient.Client.GetAsync("api/Product/");
            var product = JsonConvert.DeserializeObject<List<Product>>(await response.Content.ReadAsStringAsync())?.First();
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Попередній", "previous"),
                    InlineKeyboardButton.WithCallbackData("Наступний ➡️", "next")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("🛒 Додати до корзини", "add_to_cart")
                }
            });
            var photo = await RequestClient.Client.GetAsync(product?.Thumbnails?.First()?.URI ?? "");
            var discountPrice = product?.Price - product?.Price *
                product?.Discounts?.Where(discount => discount.Status == DiscountStatus.Active).Select(discount => discount.NormalizedDiscount).Sum(); 

            await client.SendPhotoAsync(message.Chat.Id, parseMode: ParseMode.Html,
                photo: InputFile.FromStream(photo.Content.ReadAsStream()),
                caption: $"<b>{product?.Name} [{product?.Id}]</b>\n\n" +
                         $"{product?.Description}\n\n" +
                         (discountPrice < product?.Price ? $"<b>Ціна:</b><s> {product?.Price} </s>\t{discountPrice}" : $"<b>Ціна:</b> {product?.Price}"),
                replyMarkup: keyboard);
        }

        public override Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            throw new NotImplementedException();
        }
    }
}
