using Bot.Commands;
using Bot.HttpInfrastructure;
using Infrastructure.StateMachine;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Base
{
    public class TelegramBotHandlers : ITelegramBotHandlers
    {
        private readonly List<Command> _commands;
        private readonly List<Command> _stateCommands;

        public TelegramBotHandlers()
        {
            _commands = new List<Command>
            {
                new StartCommand(),
                new ReplyCommand(),
                new GetAllProductsCommand(),
                new GetNextProductCommand(),
                new GetPreviousProductCommand(),
                new AddProductToCartCommand(),
                new GetCartCommand(),
                new GetNextProductFromCartCommand(),
                new GetPreviousProdcutFromCartCommand(),
                new RemoveAllProductItemsFromCartCommand(),
                new RemoveOneProductItemFromCartCommand(),
                new StartOrderingCommand()
            };
            _stateCommands = new List<Command>()
            {
                new GetPhoneCommand(),
                new EndOrderingCommand()
            };
        }

        public async Task MessageHandlerAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        if (await StateMachineHandlerAsync(client, update))
                        {
                            return;
                        } 

                        await MessageHandlerAsync(client, update.Message ?? throw new Exception("Message can't be null"));
                    }return;
                case  UpdateType.CallbackQuery:
                    {
                        if (await StateMachineHandlerAsync(client, update))
                        {
                            return;
                        }

                        await MessageHandlerAsync(client, update.CallbackQuery ?? throw new Exception("Callback query can't be null"));
                    }return;
                default:
                    {

                    }return;
            }
        }

        public Task ErrorHandlerAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram Bot API excepton:\n {apiRequestException.ErrorCode}\n {apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);

            return Task.CompletedTask;
        }

        private async Task MessageHandlerAsync(ITelegramBotClient client, Message message)
        {
            foreach (var el in _commands)
            {
                await el.TryExecute(client, message);
            }
        }

        private async Task MessageHandlerAsync(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            foreach (var el in _commands)
            {
                await el.TryExecute(client, callbackQuery);
            }
        }

        private async Task<bool> StateMachineHandlerAsync(ITelegramBotClient client, Update update)
        {
            var userId = update.Message == null ? update!.CallbackQuery!.From.Id : update!.Message!.From!.Id;
            var isUserHasStateResponse = await RequestClient.Client.GetAsync($"api/StateMachine/state_machine/{userId}/is-has-state");

            if (await isUserHasStateResponse.Content.ReadAsStringAsync() == string.Empty)
            {
                return false;
            }

            var isUserHasState = JsonConvert.DeserializeObject<bool>(await isUserHasStateResponse.Content.ReadAsStringAsync());

            if (isUserHasState)
            {
                var stateResponse = await RequestClient.Client.GetAsync($"api/StateMachine/state_machine/{userId}/state");
                var state = JsonConvert.DeserializeObject<State>(await stateResponse.Content.ReadAsStringAsync());

                await StateMachineHandlerAsync(client, update, state?.CurrentState ?? "");
            }

            return isUserHasState;
        }

        private async Task StateMachineHandlerAsync(ITelegramBotClient client, Update update, string state)
        {
            switch (state)
            {
                case "get_phone":
                    {
                        await _stateCommands!.Where(c => c.GetType() == typeof(GetPhoneCommand)).First().TryExecute(client, update.Message!);
                    }return;
                case "get_address":
                    {
                        await _stateCommands!.Where(c => c.GetType() == typeof(EndOrderingCommand)).First().TryExecute(client, update.Message!);
                    }
                    return;
                default:
                    {

                    }return;
            }
        }
    }
}
