using System.Threading.Tasks;
using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.Services.Bot.Interfaces
{
    public interface IExecutorMenu
    {
        Task StartMenu(Executor executor, Update update);
        Task ProcessCallBack(Executor executor, Update update);
        Task ProcessReply(Executor executor, Update update);
    }
}