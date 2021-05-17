using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Bot.Interfaces
{
    public interface IClientMenu
    {
        Task ClientMainMenu(Client thisClient, Message receivedMessage);
    }
}