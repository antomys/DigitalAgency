using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.Services.Bot.Interfaces
{
    public interface IProcessCallbacks
    {
        Task ProcessCreateOrderCallback(Update update);
        Task ProcessCallback(Update update);
    }
}