using Bot.Estensions;
using Bot.HttpInfrastructure;
using Core.Entities;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class GetCartCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "🛒 Мій кошик" };

        public override async Task Execute(ITelegramBotClient client, Message message)
        {
            var response = await RequestClient.Client.GetAsync($"api/Cart/{message.From?.Id}");

            if (await response.Content.ReadAsStringAsync() == string.Empty)
            {
                return;
            }

            var cartDictionary = JsonConvert.DeserializeObject<List<int>>(await response.Content.ReadAsStringAsync())?.ToCartDictionary();
            var cartString = $"Ваш кошик:\n\n";
            var index = 1;
            float? totalAmount = 0;

            foreach (var el in cartDictionary ?? new())
            {
                var productResponse = await RequestClient.Client.GetAsync($"api/Product/{el.Key}");
                var product = JsonConvert.DeserializeObject<Product>(await productResponse.Content.ReadAsStringAsync());
                
                cartString += $"{index}\\. *{product?.Name}*\tx{el.Value}\n";
                totalAmount += el.Value * product?.Price;
                index++;
            }

            cartString += $"\nЗагальна вартість: *{totalAmount}*";

            await client.SendTextMessageAsync(message.Chat.Id, cartString, parseMode: ParseMode.MarkdownV2);
        }

        public override Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            throw new NotImplementedException();
        }
    }
}
