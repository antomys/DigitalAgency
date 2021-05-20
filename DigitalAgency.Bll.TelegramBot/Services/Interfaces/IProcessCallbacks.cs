using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.TelegramBot.Services.Interfaces
{
    public interface IProcessCallbacks
    {
        Task ProcessCreateOrderCallback(Update update);
        Task ProcessCallback(Update update);
    }
}