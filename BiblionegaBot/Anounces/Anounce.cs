using System;

namespace BiblionegaBot.Anounces
{
    public class Anounce
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }
        public AnounceCategory Category { get; set; }

        public static AnounceCategory GetAnounceCategory(Anounce anounce)
        {
            if (anounce.Title.ToLower().Contains(" рэс") || anounce.Message.ToLower().Contains(" электроэнерги"))
            {
                return AnounceCategory.Electric;
            }
            if (anounce.Title.ToLower().Contains("водоканал") || anounce.Message.ToLower().Contains("водоканал"))
            {
                return AnounceCategory.Water;
            }
            if (anounce.Title.ToLower().Contains("онега-энергия") || anounce.Message.ToLower().Contains(" теплоснабжени") || anounce.Message.ToLower().Contains("онега-энергия"))
            {
                return AnounceCategory.Heating;
            }
            if (anounce.Message.ToLower().Contains(" угмс"))
            {
                return AnounceCategory.Weather;
            }
            if (anounce.Message.ToLower().Contains(" движен") && anounce.Message.ToLower().Contains(" автотранспорт"))
            {
                return AnounceCategory.RoadIssue;
            }
            if (anounce.Message.ToLower().Contains("ржд"))
            {
                return AnounceCategory.Railway;
            }

            return AnounceCategory.Default;
        }
    }
}
