using Bot.HttpInfrastructure;
using Core.Entities;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class GetPreviousProductCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "previous" };

        public override Task Execute(ITelegramBotClient client, Message message)
        {
            throw new NotImplementedException();
        }

        public override async Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            var mathes = Regex.Match(callbackQuery.Message?.Caption ?? "", @"\[\d+\]");
            var id = mathes.Captures.First().Value.Replace("[", "").Replace("]", "");
            var response = await RequestClient.Client.GetAsync($"api/Product/{id}/previous");
            var product = JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync());

            if (product?.Id == int.Parse(id))
            {
                return;
            }
            else
            {
                var photo = await RequestClient.Client.GetAsync(product?.Thumbnails?.First()?.URI ?? "");
                var discountPrice = product?.Price - product?.Price *
                    product?.Discounts?.Where(discount => discount.Status == DiscountStatus.Active).Select(discount => discount.NormalizedDiscount).Sum();

                await client.EditMessageMediaAsync(callbackQuery.Message?.Chat ?? throw new Exception("Chat can't be null"),
                        callbackQuery.Message.MessageId, media: new InputMediaPhoto(InputFile.FromStream(photo.Content.ReadAsStream(), "photo")));
                await client.EditMessageCaptionAsync(callbackQuery.Message?.Chat ?? throw new Exception("Chat can't be null"),
                        callbackQuery.Message.MessageId, parseMode: ParseMode.Html,
                        caption: $"<b>{product?.Name} [{product?.Id}]</b>\n\n" +
                                 $"{product?.Description}\n\n" +
                                 (discountPrice < product?.Price ? $"<b>Ціна:</b><s> {product?.Price} </s>\t{discountPrice}" : $"<b>Ціна:</b> {product?.Price}"),
                        replyMarkup: callbackQuery.Message.ReplyMarkup);
            }
        }
    }
}
