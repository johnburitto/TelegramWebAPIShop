using Bot.Base;

var bot = new TelegramBot<TelegramBotHandlers>();

bot.InitBot();
bot.StartReceiving();