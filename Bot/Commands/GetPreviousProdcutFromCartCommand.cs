using Bot.Estensions;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Core.Entities;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class GetPreviousProdcutFromCartCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "cart_prv" };

        public override Task Execute(ITelegramBotClient client, Message message)
        {
            throw new NotImplementedException();
        }

        public override async Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            var mathes = Regex.Match(callbackQuery.Message?.Caption ?? "", @"\[\d+\]");
            var id = mathes.Captures.First().Value.Replace("[", "").Replace("]", "");
            var cartResponse = await RequestClient.Client.GetAsync($"api/Cart/{callbackQuery.From?.Id}");
            var cartDictionary = JsonConvert.DeserializeObject<List<int>>(await cartResponse.Content.ReadAsStringAsync())?.ToCartDictionary();
            var productId = cartDictionary?.GetPreviousProduct(int.Parse(id));

            if (productId == int.Parse(id))
            {
                return;
            }

            var productResponse = await RequestClient.Client.GetAsync($"api/Product/{productId}");
            var product = JsonConvert.DeserializeObject<Product>(await productResponse.Content.ReadAsStringAsync());
            var photo = await RequestClient.Client.GetAsync(product?.Thumbnails?.First()?.URI ?? "");

            await client.EditMessageMediaAsync(callbackQuery.Message?.Chat ?? throw new Exception("Chat can't be null"),
                    callbackQuery.Message.MessageId, media: new InputMediaPhoto(InputFile.FromStream(photo.Content.ReadAsStream(), "photo")));
            await client.EditMessageCaptionAsync(callbackQuery.Message?.Chat ?? throw new Exception("Chat can't be null"),
                    callbackQuery.Message.MessageId, parseMode: ParseMode.Html,
                    caption: $"<b>{product?.Name} [{product?.Id}]</b> x{cartDictionary?.Where(el => el.Key == product?.Id).First().Value}\n\n" +
                             $"{product?.Description}\n\n",
                    replyMarkup: callbackQuery.Message.ReplyMarkup);
        }
    }
}
