using System.ComponentModel;

namespace BiblionegaBot.Anounces
{
    public enum AnounceCategory
    {
        //[Description("\U0001F3AB")] //ticket
        //[Description("\U0001F4AC")] //speech balloon
        [Description("\U00002709")] //envelope
        Default,
        [Description("\U000026A1")] //high voltage sign
        Electric,
        [Description("\U0001F3ED")] //factory
        Heating,
        [Description("\U00002614")] //umbrella with rain drops
        Weather,
        [Description("\U0001F6A7")] //Construction
        RoadIssue,
        [Description("\U0001F682")] //Locomotive
        Railway
    }
}
