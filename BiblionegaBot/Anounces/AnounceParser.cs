using AngleSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AngleSharp.Dom;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace BiblionegaBot.Anounces
{
    internal class AnounceParser : IAnounceParser
    {
        private readonly string _siteAddress;
        private readonly string _anouncesPath;
        private readonly Regex dateTimeRegex = new Regex("\\d{2}\\.\\d{2}\\.\\d{4}\\s\\d{2}:\\d{2}:\\d{2}");
        private readonly ILogger<AnounceParser> _logger;
        private readonly HttpClient _httpClient;

        public AnounceParser(string siteAddress, string anouncesPath, ILogger<AnounceParser> logger)
        {
            _siteAddress = siteAddress;
            _anouncesPath = anouncesPath;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<Anounce>> ParseAnouncesAsync()
        {
            _logger.LogInformation("Try to parse anounces");
            var address = _siteAddress + _anouncesPath;
            var document = await GetDocumentAsync(address).ConfigureAwait(false);
            if(document.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogWarning("Document status: {Status}", document.StatusCode);

                return null;
            }
            var anounceSelector = "div.news__item";
            var anounceNodes = document.QuerySelectorAll(anounceSelector);
            var anounces = new List<Anounce>();
            foreach (var anounceNode in anounceNodes) 
            {
                anounces.Add(await ParseAnounceAsync(anounceNode).ConfigureAwait(false));
            }

            return anounces.OrderBy(a => a.Id);
        }
                
        private async Task<IDocument> GetDocumentAsync(string address)
        {
            using var documentStream = await _httpClient.GetStreamAsync(address).ConfigureAwait(false);
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            return await context.OpenAsync(req => req.Content(documentStream)).ConfigureAwait(false);
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
                        
            var anounceDocument = await GetDocumentAsync(anounce.Link).ConfigureAwait(false);

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

        private AnounceCategory GetAnounceCategory(Anounce anounce)
        {
            if(anounce.Title.ToLower().Contains(" рэс") || anounce.Message.ToLower().Contains(" электроэнерги"))
            {
                return AnounceCategory.Electric;
            }
            if(anounce.Title.ToLower().Contains("онега-энергия") || anounce.Message.ToLower().Contains(" теплоснабжени") || anounce.Message.ToLower().Contains("онега-энергия"))
            {
                return AnounceCategory.Heating;
            }
            if(anounce.Message.ToLower().Contains(" угмс"))
            {
                return AnounceCategory.Weather;
            }
            if(anounce.Message.ToLower().Contains(" движен") && anounce.Message.ToLower().Contains(" автотранспорт"))
            {
                return AnounceCategory.RoadIssue;
            }
            if(anounce.Message.ToLower().Contains("ржд"))
            {
                return AnounceCategory.Railway;
            }

            return AnounceCategory.Default;

        }
    }
}
