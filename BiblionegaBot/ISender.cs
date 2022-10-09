using BiblionegaBot.Anounces;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BiblionegaBot
{
    public interface ISender
    {
        Task<User> GetBotInfoAsync();
        Task<Message> SendAnounceAsync(Anounce anounce, string chatId);
    }
}
