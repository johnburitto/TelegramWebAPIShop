using Bot.Estensions;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Bot.StateObjects;
using Core.Dtos.Create;
using Infrastructure.StateMachine;
using Newtonsoft.Json;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands
{
    public class EndOrderingCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "" };

        public override async Task Execute(ITelegramBotClient client, Message message)
        {
            if(message.Text != null &&  message.Text != string.Empty)
            {
                var stateResponse = await RequestClient.Client.GetAsync($"api/StateMachine/state_machine/{message.From?.Id}/state");
                var cartResponse = await RequestClient.Client.GetAsync($"api/Cart/{message.From?.Id}");
                var cart = JsonConvert.DeserializeObject<List<int>>(await cartResponse.Content.ReadAsStringAsync());
                var state = JsonConvert.DeserializeObject<State>(await stateResponse.Content.ReadAsStringAsync());
                
                state!.StateObject = JsonConvert.DeserializeObject<PhoneDto>(state!.StateObject!.ToString() ?? "");

                var orederDto = new OrderCreateDto()
                {
                    UserTelegramId = message.From?.Id.ToString(),
                    Name = $"{message.From?.FirstName} {message.From?.LastName}",
                    Phone = (state!.StateObject as PhoneDto)!.Phone,
                    Address = message.Text,
                    TotalPrise = await cart!.ToCartDictionary().TotalPriceAsync(),
                    ProductsIds = cart
                };
                var orderDtoString = JsonConvert.SerializeObject(orederDto);
                var orderDtoData = new StringContent(orderDtoString, Encoding.UTF8, "application/json");

                await RequestClient.Client.PostAsync("api/Order", orderDtoData);
                await RequestClient.Client.DeleteAsync($"api/StateMachine/state_machine/{message.From?.Id}");
                await RequestClient.Client.DeleteAsync($"api/Cart/{message.From?.Id}");
                await client.SendTextMessageAsync(message!.Chat.Id, "✅ Замовлення успішно сформовано");
            }
            else
            {
                await client.SendTextMessageAsync(message!.Chat.Id, "🗺️ Введіть адресу");
            }
        }

        public override Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            throw new NotImplementedException();
        }
    }
}
