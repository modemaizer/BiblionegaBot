﻿using BiblionegaBot.Anounces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace BiblionegaBot
{
    internal class Worker
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAnounceParser _anounceParser;
        private readonly IDataLayer _dataLayer;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly ITelegramBotClient _botClient;

        public Worker(
            ILogger<Worker> logger, 
            IAnounceParser anounceParser, 
            IDataLayer dataLayer,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _logger = logger;
            _anounceParser = anounceParser;
            _dataLayer = dataLayer;
            _configuration = configuration;
            _botClient = new TelegramBotClient(configuration["BotApiKey"]);
        }

        internal async Task RunAsync()
        {
            var chatId = _configuration["ChatId"];
            _logger.LogInformation("Start {BotName} for {ChatId}", (await _botClient.GetMeAsync().ConfigureAwait(false)).FirstName, chatId);
            var anounces = await _anounceParser.ParseAnouncesAsync().ConfigureAwait(false);
            var lastStoredAnounce = _dataLayer.GetLastStoredAnounce();
            if (lastStoredAnounce != null)
            {
                anounces = anounces.Where(a => a.Id > lastStoredAnounce.Id).ToList();
            }
            foreach (var anounce in anounces)
            {
                if (_dataLayer.SaveAnounce(anounce))
                {
                    var message = $"<b>{anounce.Title}</b>\n" + 
                        $"<b>{anounce.Created.ToString("dd.MM.yyyy HH:mm")}</b>\n" +                        
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

                        if(result == null)
                        {
                            _logger.LogWarning("Anounce {AnounceId} wasn't sent to chat {ChatId}", anounce.Id, chatId);
                        }
                    }
                    catch(Exception exception)
                    {
                        _logger.LogError(exception, "Sending anounce {AnounceId} to chat {ChatId} failed", anounce.Id, chatId);
                    }
                }
            }
        }
    }
}
