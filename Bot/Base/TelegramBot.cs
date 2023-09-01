using Bot.Utils;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Bot.Base
{
    public class TelegramBot<THandlers> where THandlers : ITelegramBotHandlers, new()
    {
        protected TelegramBotClient? Bot { get; set; }
        protected ReceiverOptions? ReceiverOptions { get; set; }
        protected THandlers Handlers => new THandlers();
        protected string Token => UsersecretsReader.ReadSection<string>("Token");
        protected CancellationTokenSource CancellationTokenSource => new CancellationTokenSource();

        public void InitBot()
        {
            Bot = new TelegramBotClient(Token);
            ReceiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };
        }

        public void StartReceiving()
        {
            Bot?.StartReceiving(
                Handlers.MessageHandlerAsync,
                Handlers.ErrorHandlerAsync,
                ReceiverOptions,
                CancellationTokenSource.Token);

            Console.ReadKey();
            CancellationTokenSource.Cancel();
        }
    }
}
