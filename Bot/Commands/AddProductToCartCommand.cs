using Bot.HttpInfrastructure;
using Core.Dtos.Create;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands
{
    public class AddProductToCartCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "add_to_cart" };

        public override Task Execute(ITelegramBotClient client, Message message)
        {
            throw new NotImplementedException();
        }

        public override async Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            var response = await RequestClient.Client.GetAsync($"api/Cart/{callbackQuery.From.Id}");

            if (await response.Content.ReadAsStringAsync() == string.Empty)
            {
                var dto = new CartCreateDto
                {
                    UserTelegramId = callbackQuery.From.Id,
                    ExpirationTimeInHours = 3
                };
                var dtoJson = JsonConvert.SerializeObject(dto);
                var dtoData = new StringContent(dtoJson, Encoding.UTF8, "application/json");

                await RequestClient.Client.PostAsync("api/Cart", dtoData);
            }

            var mathes = Regex.Match(callbackQuery.Message?.Caption ?? "", @"\[\d+\]");
            var id = mathes.Captures.First().Value.Replace("[", "").Replace("]", "");

            await RequestClient.Client.PostAsync($"api/Cart/add/{callbackQuery.From.Id}/{id}", null);
        }
    }
}
