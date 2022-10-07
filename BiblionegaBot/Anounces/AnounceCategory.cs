using System.ComponentModel;

namespace BiblionegaBot.Anounces
{
    public enum AnounceCategory
    {
        [Description("\U0001F3AB")]   //ticket
        Default,
        [Description("\U000026A1")]   //high voltage sign
        Electric,
        [Description("\U0001F3ED")]   //factory
        Heating,
        [Description("\U00002614")]   //umbrella with rain drops
        Weather
    }
}
