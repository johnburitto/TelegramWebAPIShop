using Bot.HttpInfrastructure;
using Bot.StateObjects;
using Infrastructure.StateMachine;
using Newtonsoft.Json;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands
{
    public class StartOrderingCommand : Command
    {
        protected override List<string> Names { get; set; } = new() { "start_ordering" };

        public override Task Execute(ITelegramBotClient client, Message message)
        {
            throw new NotImplementedException();
        }

        public override async Task Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            var state = new State
            {
                UserTelegramId = callbackQuery.From.Id,
                CurrentState = "get_phone",
                StateObject = new PhoneDto()
            };
            var stateString = JsonConvert.SerializeObject(state);
            var stateData = new StringContent(stateString, Encoding.UTF8, "application/json");

            await RequestClient.Client.PostAsync("api/StateMachine/add/state_machine", stateData);
            await client.SendTextMessageAsync(callbackQuery!.Message!.Chat.Id, "📞 Введіть номер телефону");
        }
    }
}
