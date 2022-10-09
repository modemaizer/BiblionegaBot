using BiblionegaBot.Anounces;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BiblionegaBot
{
    internal interface ISender
    {
        Task<User> GetBotInfoAsync();
        Task<Message> SendAnounceAsync(Anounce anounce, string chatId);
    }
}
