using Bot.HttpInfrastructure;
using Bot.StateObjects;
using Infrastructure.StateMachine;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands
{
    public class GetPhoneCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "" };

        public override async Task Execute(ITelegramBotClient client, Message message)
        {
            var match = Regex.Match(message.Text ?? "", @"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$");

            if (match.Value != string.Empty && match.Value != null)
            {
                var stateResponse = await RequestClient.Client.GetAsync($"api/StateMachine/state_machine/{message.From?.Id}/state");
                var state = JsonConvert.DeserializeObject<State>(await stateResponse.Content.ReadAsStringAsync());
                
                state!.StateObject = JsonConvert.DeserializeObject<PhoneDto>(state!.StateObject!.ToString() ?? "");
                (state!.StateObject as PhoneDto)!.Phone = match.Value;
                state.CurrentState = "get_address";

                var stateString = JsonConvert.SerializeObject(state);
                var stateData = new StringContent(stateString, Encoding.UTF8, "application/json");

                await RequestClient.Client.PostAsync("api/StateMachine/change/state_machine", stateData);
                await client.SendTextMessageAsync(message!.Chat.Id, "🗺️ Введіть адресу");
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, "📞 Введіть номер телефону");
            }
        }

        public override Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            throw new NotImplementedException();
        }
    }
}
