using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiblionegaBot.Anounces
{
    public interface IAnounceParser
    {
        Task<IEnumerable<Anounce>> ParseAnouncesAsync();

        Task ParseAnounceDetailsAsync(Anounce anounce);
    }
}
