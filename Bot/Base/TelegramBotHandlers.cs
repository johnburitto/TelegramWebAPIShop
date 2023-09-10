using Bot.Commands;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Base
{
    public class TelegramBotHandlers : ITelegramBotHandlers
    {
        private readonly List<Command> _commands;

        public TelegramBotHandlers()
        {
            _commands = new List<Command>
            {
                new StartCommand(),
                new ReplyCommand(),
                new GetAllProductsCommand(),
                new GetNextProductCommand(),
                new GetPreviousProductCommand()
            };
        }

        public async Task MessageHandlerAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    await MessageHandlerAsync(client, update.Message ?? throw new Exception("Message can't be null"));
                }break;
                case  UpdateType.CallbackQuery:
                {
                    await MessageHandlerAsync(client, update.CallbackQuery ?? throw new Exception("Callback query can't be null"));
                }break;
                default:
                {
                
                }break;
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
    }
}
