using BiblionegaBot.Anounces;
using BiblionegaBot.Extensions;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using System.Text.RegularExpressions;

namespace BiblionegaBot
{
    public class Sender : ISender
    {
        private static readonly Regex _boldRegex = new("(?:\\d{1,2}\\.\\d{1,2}\\.\\d{4}г?|\\d{2}:\\d{2}|\\d{1,2} (?:января|февраля|марта|апреля|мая|июня|июля|августа|сентября|октября|ноября|декабря)|(?:с|с|до|по) \\d{2}:\\d{2})");

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

        private static string NormalizeMessage(string message)
        {
            message = message.Replace("&nbsp;", " ");
            message = Regex.Replace(message, "\\s\\s+", " ");
            message = Regex.Replace(message, ":{2,}", ":");
            return message.Trim();
        }

        private static string BuildMessage(Anounce anounce)
        {
            var title = $"<b>{anounce.Category.GetDescription()} {anounce.Title}</b>";
            var date = $"[{anounce.Created:dd.MM.yyyy HH:mm}]";
            var message = _boldRegex.Replace(NormalizeMessage(anounce.Message), m => "<b>" + m.Value + "</b>");

            return title + "\n" + date + "\n<i>" + message+"</i>";
        }

        public async Task<Message> SendAnounceAsync(Anounce anounce, string chatId)
        {
            var message = BuildMessage(anounce);
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
