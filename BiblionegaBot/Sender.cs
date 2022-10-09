using BiblionegaBot.Anounces;
using BiblionegaBot.Extensions;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace BiblionegaBot
{
    public class Sender : ISender
    {
        private readonly ILogger<Sender> _logger;
        private readonly ITelegramBotClient _botClient;

        public Sender(ILogger<Sender> logger, ITelegramBotClient botClient)
        {
            _logger = logger;
            _botClient = botClient;
        }

        public Task<User> GetBotInfoAsync()
        {
            return _botClient.GetMeAsync();
        }

        public async Task<Message> SendAnounceAsync(Anounce anounce, string chatId)
        {
            var message = $"<b>{anounce.Category.GetDescription()} {anounce.Title}</b>\n" +
                $"<b>{anounce.Created:dd.MM.yyyy HH:mm}</b>\n" +
                $"<i>{anounce.Message}</i>";
            try
            {
                var result = await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    replyMarkup: new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithUrl(
                            "Посмотреть на сайте",
                            anounce.Link))
                ).ConfigureAwait(false);

                if (result == null)
                {
                    _logger.LogWarning("Anounce {AnounceId} wasn't sent to chat {ChatId}", anounce.Id, chatId);
                }

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Sending anounce {AnounceId} to chat {ChatId} failed", anounce.Id, chatId);
                return null;
            }
        }
    }
}
