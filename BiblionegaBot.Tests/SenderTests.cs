using BiblionegaBot.Anounces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework.Internal;
using Telegram.Bot;

namespace BiblionegaBot.Tests;

public class SenderTests
{
    private const string BotApiKey = "5767713276:AAFyMpGHOKJkUVMasVTU_ejI_-GZS19RqLw";
    private ISender _sender;

    [SetUp]
    public void Setup()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        var logger = factory.CreateLogger<Sender>();
        _sender = new Sender(logger, new TelegramBotClient(BotApiKey));
    }

    [Test]
    public async Task SendAnounceAsync_Test()
    {
        var anounce = new Anounce
        {
            Id = 123,
            Title = "Test Anounce Title",
            Category = AnounceCategory.RoadIssue,
            Message = "Test Anounce Message",
            Link = "http://onegaland.ru",
            Created = DateTime.Now
        };

        var sendResult = await _sender.SendAnounceAsync(anounce, "@biblionega_test").ConfigureAwait(false);

        Assert.IsNotNull(sendResult);
    }
}