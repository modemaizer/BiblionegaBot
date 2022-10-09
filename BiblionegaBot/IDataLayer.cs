using BiblionegaBot.Anounces;
using Telegram.Bot.Types;

namespace BiblionegaBot
{
    public interface IDataLayer
    {
        Anounce GetLastStoredAnounce();
        bool SaveAnounce(Anounce anounce);
        bool SaveUser(User user);
    }
}
