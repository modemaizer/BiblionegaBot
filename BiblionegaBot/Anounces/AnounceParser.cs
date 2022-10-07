using AngleSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AngleSharp.Dom;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace BiblionegaBot.Anounces
{
    internal class AnounceParser : IAnounceParser
    {
        private readonly string _siteAddress;
        private readonly string _anouncesPath;
        private readonly IBrowsingContext _context;
        private readonly Regex dateTimeRegex = new Regex("\\d{2}\\.\\d{2}\\.\\d{4}\\s\\d{2}:\\d{2}:\\d{2}");
        private readonly ILogger<AnounceParser> _logger;

        public AnounceParser(string siteAddress, string anouncesPath, ILogger<AnounceParser> logger)
        {
            _siteAddress = siteAddress;
            _anouncesPath = anouncesPath;
            var config = Configuration.Default.WithDefaultLoader();
            _context = BrowsingContext.New(config);
            _logger = logger;            
        }

        public async Task<IEnumerable<Anounce>> ParseAnouncesAsync()
        {
            _logger.LogInformation("Try to parse anounces");
            var address = _siteAddress + _anouncesPath;
            var document = await _context.OpenAsync(address).ConfigureAwait(false);
            var anounceSelector = "div.news__item";
            var anounceNodes = document.QuerySelectorAll(anounceSelector);
            var anounces = new List<Anounce>();
            foreach (var anounceNode in anounceNodes) 
            {
                anounces.Add(await ParseAnounceAsync(anounceNode).ConfigureAwait(false));
            }

            return anounces.OrderBy(a => a.Id);
        }

        private async Task<Anounce> ParseAnounceAsync(IElement anounceNode)
        {
            var anounce = new Anounce();
            var idText = anounceNode.Id;
            anounce.Id = int.Parse(idText.Substring(idText.LastIndexOf('_') + 1));
            var titleNode = anounceNode.QuerySelector("a.news__item__title");
            anounce.Title = titleNode.Text().Trim();
            anounce.Link = _siteAddress + titleNode.GetAttribute("href");
            anounce.Message = anounceNode.QuerySelector("div.news__item__text").Text().Trim();
            anounce.Category = GetAnounceCategory(anounce);

            var anounceDocument = await _context.OpenAsync(anounce.Link).ConfigureAwait(false);

            var detailText = anounceDocument.QuerySelector("div.news-detail").Text().Trim();
            var match = dateTimeRegex.Matches(detailText).Last();
            if(match != null && DateTime.TryParse(match.Value, out var createdDateTime))
            {
                anounce.Created = createdDateTime;
            }
            else if(DateTime.TryParse(anounceNode.QuerySelector("div.news__item__date").Text().Trim(), out var createdDate))
            {
                anounce.Created = createdDate;
            }
            else
            {
                anounce.Created = DateTime.Now;
            }

            return anounce;
        }
    }
}
