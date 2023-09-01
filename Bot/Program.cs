using Bot.Base;
using Telegram.Bot;
var bot = new TelegramBot<TelegramBotHandlers>();

bot.InitBot();
bot.StartReceiving();