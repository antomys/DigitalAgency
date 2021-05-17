using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Bot.Interfaces
{
    public interface IWelcomeMenu
    {
        Task StartMenu(Client thisClient, Message receivedMessage);
    }
}