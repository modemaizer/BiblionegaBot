using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiblionegaBot.Anounces
{
    internal interface IAnounceParser
    {
        Task<IEnumerable<Anounce>> ParseAnouncesAsync();
    }
}
