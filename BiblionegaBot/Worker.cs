using BiblionegaBot.Anounces;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace BiblionegaBot
{
    internal class Worker
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAnounceParser _anounceParser;
        private readonly ISender _sender;
        private readonly IDataLayer _dataLayer;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public Worker(
            ILogger<Worker> logger, 
            IAnounceParser anounceParser,
            ISender sender,
            IDataLayer dataLayer,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _logger = logger;
            _anounceParser = anounceParser;
            _sender = sender;
            _dataLayer = dataLayer;
            _configuration = configuration;
        }

        internal async Task RunAsync(bool silent)
        {
            var chatId = _configuration["ChatId"];
            _logger.LogInformation("Start {BotName} for {ChatId}", (await _sender.GetBotInfoAsync().ConfigureAwait(false)).FirstName, chatId);
            var anounces = await _anounceParser.ParseAnouncesAsync().ConfigureAwait(false);
            if (anounces == null)
            {
                return;
            }
            _logger.LogInformation("{Count} anounces were found", anounces.Count());
            var lastStoredAnounce = _dataLayer.GetLastStoredAnounce();
            if (lastStoredAnounce != null)
            {
                anounces = anounces.Where(a => a.Id > lastStoredAnounce.Id).ToList();
            }
            var count = 0;
            foreach (var anounce in anounces)
            {
                await _anounceParser.ParseAnounceDetailsAsync(anounce).ConfigureAwait(false);
                if(_dataLayer.SaveAnounce(anounce) && 
                    !silent && 
                    await _sender.SendAnounceAsync(anounce, chatId).ConfigureAwait(false) != null)
                {
                    count++;
                }
            }
            _logger.LogInformation("{Count} anounces were sent", count);
        }
    }
}
