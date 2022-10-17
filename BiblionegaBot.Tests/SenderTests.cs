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
            Title = "Сообщение от ООО \"Водоканал\"",
            Message = "Потребителям г. Онега (ул. Кр. Курсантов,18/7, пр. Ленина, дос 172,176, пр. Ленина на участке Победы-К. Маркса). В связи с работами на ТП-38 фидер №№3,5,7,КНС-1 будет прекращена подача электроэнергии 12.10.2022г., в период с 03:00 до 17:00.",
            //"АО \"Онега-Энергия\" информирует: в связи с проведением ремонтных работ на теплосети квартал \"Рочево\" будет прекращено теплоснабжение по адресам: Рочевская 1а, Новая 1,2,3,4,5,8, пер. Рочевский 1,3,4,5, Ленина 207, 207а, 207б, 209, Парковая 1,5, Наб. Комарова 62,62а,62б,64, 11.10.2022г. с 09:00 до завершения работ.",
            //"По сообщению дисп. ООО \" Водоканал\" в связи с плановыми работами, будет остановлен-головной забор и резервная насосная станция (ул. Пионерская) 16.10.2022г. с  06::00 до 13:00 будет прекращена подача ХВС  ( город )",
            Link = "http://onegaland.ru",
            Created = DateTime.Now
        };
        anounce.Category = Anounce.GetAnounceCategory(anounce);

        var sendResult = await _sender.SendAnounceAsync(anounce, "@biblionega_test").ConfigureAwait(false);

        Assert.IsNotNull(sendResult);
    }
}