using Bot.HttpInfrastructure;
using Core.Entities;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class GetAllProductsCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "🎁 Всі товари" };

        public override async Task Execute(ITelegramBotClient client, Message message)
        {
            var response = await RequestClient.Client.GetAsync("api/Product/");
            var produts = JsonConvert.DeserializeObject<List<Product>>(await response.Content.ReadAsStringAsync());

            foreach (var el in produts ?? new())
            {
                var photo = await RequestClient.Client.GetAsync(el.Thumbnails?.First()?.URI ?? "");

                await client.SendPhotoAsync(message.Chat.Id, parseMode: ParseMode.MarkdownV2,
                    photo: InputFile.FromStream(photo.Content.ReadAsStream()),
                    caption: $"*Назва:* {el.Name}\n" + 
                             $"*Опис:* {el.Description}\n" + 
                             $"*Ціна:* {el.Price}");
            } 
        }
    }
}
