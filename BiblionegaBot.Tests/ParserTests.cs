using BiblionegaBot.Anounces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace BiblionegaBot.Tests
{
    public class ParserTests
    {
        private const string SiteAddress = "http://onegaland.ru";
        private const string AnouncesPath = "/about/anounces/";
        private IAnounceParser _parser;

        [SetUp]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<AnounceParser>();
            _parser = new AnounceParser(SiteAddress, AnouncesPath, logger);
        }

        [Test]
        public async Task ParseAnounceDetails_Test()
        {
            var anounce = new Anounce
            {
                Link = "http://onegaland.ru/about/anounces/42655/",
            };

            await _parser.ParseAnounceDetailsAsync(anounce).ConfigureAwait(false);
            Assert.That(anounce, Is.Not.Null);
            Assert.That(anounce.Created, Is.EqualTo(DateTime.Parse("10.10.2022 14:14:01")));
        }

        [Test]
        public async Task ParseAnounces_Test()
        {
            var anounces = await _parser.ParseAnouncesAsync().ConfigureAwait(false);
            foreach(var anounce in anounces)
            {
                var created = anounce.Created;
                await _parser.ParseAnounceDetailsAsync(anounce).ConfigureAwait(false);
                Assert.That(anounce.Created, Is.Not.EqualTo(created));
            }

            Assert.That(anounces.Count, Is.EqualTo(10));
        }
    }
}
